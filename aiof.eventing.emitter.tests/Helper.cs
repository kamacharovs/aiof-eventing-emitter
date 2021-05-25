using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos.Table;
using Azure.Messaging.ServiceBus;

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

        #region UnitTests

        public static EventRequest GetRandomEventRequest()
        {
            return GetRandomEventRequests().First();
        }
        public static IEnumerable<EventRequest> GetRandomEventRequests(int n = 1)
        {
            return new Faker<EventRequest>()
                .RuleFor(x => x.EventType, f => f.PickRandom<EventType>().ToString())
                .RuleFor(x => x.Source, f => GetRandomEventSource())
                .RuleFor(x => x.User, f => GetRandomEventUser())
                .RuleFor(x => x.Entity, f => GetRandomEventEntity())
                .Generate(n);
        }

        public static EventSource GetRandomEventSource()
        {
            return GetRandomEventSources().First();
        }
        public static IEnumerable<EventSource> GetRandomEventSources(int n = 1)
        {
            return new Faker<EventSource>()
                .RuleFor(x => x.Api, f => f.Internet.Url())
                .RuleFor(x => x.Ip, f => f.Internet.Ip())
                .Generate(n);
        }

        public static EventUser GetRandomEventUser()
        {
            return GetRandomEventUsers().First();
        }
        public static IEnumerable<EventUser> GetRandomEventUsers(int n = 1)
        {
            return new Faker<EventUser>()
                .RuleFor(x => x.Id, f => f.Random.Int(1, 10000))
                .RuleFor(x => x.PublicKey, f => Guid.NewGuid())
                .Generate(n);
        }

        public static EventEntity GetRandomEventEntity()
        {
            return GetRandomEventEntities().First();
        }
        public static IEnumerable<EventEntity> GetRandomEventEntities(int n = 1)
        {
            return new Faker<EventEntity>()
                .RuleFor(x => x.Id, f => f.Random.Int(1, 10000))
                .RuleFor(x => x.Type, f => f.Random.String2(5, 100))
                .RuleFor(x => x.Payload, f => new { 
                    field1 = "field1",
                    field2 = f.Random.Int(1000, 10000)
                })
                .Generate(n);
        }
        #endregion
    }
}
