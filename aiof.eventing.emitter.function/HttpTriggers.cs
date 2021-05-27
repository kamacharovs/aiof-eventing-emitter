using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.function
{
    public class HttpTriggers
    {
        private readonly IEmitterRepository _repo;
        private readonly ServiceBusSender _sender;

        public HttpTriggers(
            IEmitterRepository repo,
            ServiceBusSender sender)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        [FunctionName("EmitEventAsync")]
        public async Task<IActionResult> EmitEventAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "emit")] EventRequest req)
        {
            await _sender.SendMessageAsync(await _repo.EmitAsync(req));

            return new OkResult();
        }
    }
}
