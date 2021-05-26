using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Azure.Messaging.ServiceBus;

using Moq;
using AutoMapper;
using Bogus;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.tests
{
    [ExcludeFromCodeCoverage]
    public static class Helper
    {
        public static Dictionary<string, string> ConfigurationDict
            => new Dictionary<string, string>()
        {
            { "AzureWebJobsStorage", "UseDevelopmentStorage" },
            { "FUNCTIONS_WORKER_RUNTIME", "dotnet" },
            { "StorageConnectionString", "DefaultEndpointsProtocol=https;AccountName=localtests;AccountKey=localtests==;EndpointSuffix=core.windows.net" },
            { "ServiceBusConnection", "Endpoint=sb://localtests.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=localtests" },
            { "EmitterTopicName", "emitter-topic" },
            { "EmitterConfigTableName", "EmitterConfig" },
            { "EmitterLogTableName", "EventLog" }
        };

        public static T GetRequiredService<T>()
        {
            var provider = Provider();

            return provider.GetRequiredService<T>();
        }

        private static IServiceProvider Provider()
        {
            var services = new ServiceCollection();

            services.AddScoped<IConfiguration>(x =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(ConfigurationDict);
                return configurationBuilder.Build();
            });

            services
                .AddSingleton(new ServiceBusClient(ConfigurationDict[Keys.ServiceBusConnection]))
                .AddSingleton(CloudStorageAccount.Parse(ConfigurationDict[Keys.StorageConnectionString]).CreateCloudTableClient(new TableClientConfiguration()));

            services.AddSingleton(new MapperConfiguration(x => { x.AddProfile(new EventProfile()); }).CreateMapper());

            services
                .AddScoped<IEmitterRepository, EmitterRepository>();

            services.AddLogging();

            return services.BuildServiceProvider();
        }

        public static IEmitterRepository GetEmitterRepository(string eventType)
        {
            var logger = GetRequiredService<ILogger<EmitterRepository>>();

            return new EmitterRepository(
                logger: logger,
                configRepo: GetEventConfigRepository(eventType),
                logRepo: GetEventLogRepository(eventType));
        }

        public static IEventConfigRepository GetEventConfigRepository(string eventType)
        {
            var logger = GetRequiredService<ILogger<EventConfigRepository>>();
            var mapper = GetRequiredService<IMapper>();
            var config = GetRequiredService<IConfiguration>();

            return new EventConfigRepository(
                logger: logger,
                mapper: mapper,
                config: config,
                client: GetMockedCloudTableClient(eventType));
        }

        public static IEventLogRepository GetEventLogRepository(string eventType)
        {
            var logger = GetRequiredService<ILogger<EventLogRepository>>();
            var mapper = GetRequiredService<IMapper>();
            var config = GetRequiredService<IConfiguration>();

            return new EventLogRepository(
                logger: logger,
                mapper: mapper,
                config: config,
                client: GetMockedCloudTableClient(eventType));
        }

        public static CloudTableClient GetMockedCloudTableClient(string eventType)
        {
            var config = Provider().GetRequiredService<IConfiguration>();

            var mockedConfigCloudTable = new Mock<CloudTable>(new Uri("http://unittests.localhost.com/EventConfig"), null);
            var mockedLogCloudTable = new Mock<CloudTable>(new Uri("http://unittests.localhost.com/EventLog"), null);
            var mockedCloudTableClient = new Mock<CloudTableClient>(new Uri("http://localhost"), new StorageCredentials("blah", "blah"), null);

            mockedConfigCloudTable.Setup(x => x.CreateIfNotExistsAsync())
                .ReturnsAsync(true);

            /*
             * EventConfig logic
             */
            var eventTypeFilter = TableQuery.GenerateFilterCondition(nameof(EventConfig.PartitionKey), QueryComparisons.Equal, eventType);
            var query = new TableQuery<EventConfig>().Where(eventTypeFilter);

            mockedConfigCloudTable.Setup(x => x.ExecuteQuery(It.Is<TableQuery<EventConfig>>(x => x.FilterString == query.FilterString),
                It.IsAny<TableRequestOptions>(),
                It.IsAny<OperationContext>()))
                .Returns(new List<EventConfig>
                {
                    new EventConfig
                    {
                        PartitionKey = eventType,
                        RowKey = Guid.NewGuid().ToString()
                    }
                });

            mockedCloudTableClient.Setup(x => x.GetTableReference(config[Keys.EmitterConfigTableName]))
                .Returns(mockedConfigCloudTable.Object);

            /*
             * EventLog logic
             */
            mockedLogCloudTable.Setup(x => x.ExecuteAsync(It.Is<TableOperation>(x => x.OperationType == TableOperationType.Insert)))
                .ReturnsAsync(new TableResult 
                {
                    HttpStatusCode = (int)HttpStatusCode.OK
                });

            mockedCloudTableClient.Setup(x => x.GetTableReference(config[Keys.EmitterLogTableName]))
                .Returns(mockedLogCloudTable.Object);

            return mockedCloudTableClient.Object;
        }

        #region UnitTests
        public static IEnumerable<object[]> EventRequests()
        {
            return GetRandomEventRequests().ToObjectArray();
        }
        public static EventRequest GetRandomEventRequest()
        {
            return GetRandomEventRequests(1).First();
        }
        public static IEnumerable<EventRequest> GetRandomEventRequests(int n = 3)
        {
            return new Faker<EventRequest>()
                .RuleFor(x => x.EventType, f => f.PickRandom<EventType>().ToString())
                .RuleFor(x => x.Source, f => GetRandomEventSource())
                .RuleFor(x => x.User, f => GetRandomEventUser())
                .RuleFor(x => x.Entity, f => GetRandomEventEntity())
                .Generate(n);
        }

        public static IEnumerable<object[]> EventSources()
        {
            return GetRandomEventSources().ToObjectArray();
        }
        public static EventSource GetRandomEventSource()
        {
            return GetRandomEventSources(1).First();
        }
        public static IEnumerable<EventSource> GetRandomEventSources(int n = 3)
        {
            return new Faker<EventSource>()
                .RuleFor(x => x.Api, f => f.Internet.Url())
                .RuleFor(x => x.Ip, f => f.Internet.Ip())
                .Generate(n);
        }

        public static IEnumerable<object[]> EventUsers()
        {
            return GetRandomEventUsers().ToObjectArray();
        }
        public static EventUser GetRandomEventUser()
        {
            return GetRandomEventUsers(1).First();
        }
        public static IEnumerable<EventUser> GetRandomEventUsers(int n = 3)
        {
            return new Faker<EventUser>()
                .RuleFor(x => x.Id, f => f.Random.Int(1, 10000))
                .RuleFor(x => x.PublicKey, f => Guid.NewGuid())
                .Generate(n);
        }

        public static IEnumerable<object[]> EventEntities()
        {
            return GetRandomEventEntities().ToObjectArray();
        }
        public static EventEntity GetRandomEventEntity()
        {
            return GetRandomEventEntities(1).First();
        }
        public static IEnumerable<EventEntity> GetRandomEventEntities(int n = 3)
        {
            return new Faker<EventEntity>()
                .RuleFor(x => x.Id, f => f.Random.Int(1, 10000))
                .RuleFor(x => x.Type, f => f.Random.String2(5, 100))
                .RuleFor(x => x.Payload, f => new
                {
                    field1 = "field1",
                    field2 = f.Random.Int(1000, 10000)
                })
                .Generate(n);
        }

        public static IEnumerable<object[]> EventTypes()
        {
            return new List<object[]>
            {
                new object[] { EventType.AssetAdded.ToString() }
            };
        }
        #endregion
    }
}
