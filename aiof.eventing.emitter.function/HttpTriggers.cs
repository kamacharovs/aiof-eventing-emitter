using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.function
{
    public class HttpTriggers
    {
        public readonly IEmitterRepository _repo;

        public HttpTriggers(
            IEmitterRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [FunctionName("EmitEventAsync")]
        [return: ServiceBus("%EmitterTopicName%", Connection = "%ServiceBusConnection%")]
        public async Task<string> EmitEventAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "emit")] EventRequest req)
        {
            return await _repo.EmitAsync(req);
        }
    }
}
