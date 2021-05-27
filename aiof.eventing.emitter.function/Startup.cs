using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;
using Azure.Messaging.ServiceBus;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

[assembly: FunctionsStartup(typeof(aiof.eventing.emitter.function.Startup))]
namespace aiof.eventing.emitter.function
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        private IConfiguration _config { get; set; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _config = builder.GetContext().Configuration;

            builder.Services.AddAutoMapper(typeof(EventProfile).Assembly);

            builder.Services
                .AddLogging()
                .AddSingleton(_config)
                .AddSingleton(CloudStorageAccount.Parse(_config[Keys.StorageConnectionString]).CreateCloudTableClient(new TableClientConfiguration()));

            builder.Services
                .AddScoped<IEmitterRepository, EmitterRepository>()
                .AddScoped<IEventConfigRepository, EventConfigRepository>()
                .AddScoped<IEventLogRepository, EventLogRepository>();

            builder.Services
                .AddServiceBus(_config);
        }
    }

    public static class ServiceBusExtensions
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration config)
        {
            var serviceBusClient = new ServiceBusClient(config[Keys.ServiceBusConnection]);
            var serviceBusSender = serviceBusClient.CreateSender(config[Keys.EmitterTopicName]);

            services
                .AddScoped(x => serviceBusClient)
                .AddScoped(x => serviceBusSender);

            return services;
        }
    }
}
