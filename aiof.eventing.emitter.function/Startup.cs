using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

[assembly: FunctionsStartup(typeof(aiof.eventing.emitter.function.Startup))]
namespace aiof.eventing.emitter.function
{
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
                .AddScoped<IEmitterRepository, EmitterRepository>();
        }
    }
}
