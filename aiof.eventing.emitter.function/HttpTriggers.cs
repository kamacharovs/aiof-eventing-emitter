using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Messaging.ServiceBus;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.function
{
    public class HttpTriggers
    {
        private readonly IConfiguration _config;
        private readonly IEmitterRepository _repo;

        public HttpTriggers(
            IConfiguration config,
            IEmitterRepository repo)
        {
            _config = config;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [FunctionName("EmitEventAsync")]
        public async Task EmitEventAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "emit")] EventRequest req)
        {
            await using (var client = new ServiceBusClient(_config[Keys.ServiceBusConnection]))
            {
                // create a sender for the queue 
                var sender = client.CreateSender(_config[Keys.EmitterTopicName]);

                // send the message
                await sender.SendMessageAsync(await _repo.EmitAsync(req));
            }
        }
    }
}
