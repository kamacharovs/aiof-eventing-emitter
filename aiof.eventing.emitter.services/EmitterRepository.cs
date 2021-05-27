using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EmitterRepository : IEmitterRepository
    {
        private readonly ILogger<EmitterRepository> _logger;
        private readonly IEventConfigRepository _configRepo;
        private readonly IEventLogRepository _logRepo;

        public EmitterRepository(
            ILogger<EmitterRepository> logger,
            IEventConfigRepository configRepo,
            IEventLogRepository logRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configRepo = configRepo ?? throw new ArgumentNullException(nameof(configRepo));
            _logRepo = logRepo ?? throw new ArgumentNullException(nameof(logRepo));
        }

        public async Task<ServiceBusMessage> EmitAsync(EventRequest req)
        {
            await _logRepo.LogAsync(req);

            var config = await _configRepo.GetConfigAsync(req.EventTypeEnum.ToString());

            if (config is null)
                return null;

            var reqStr = JsonConvert.SerializeObject(req, Constants.JsonSettings);
            var message = new ServiceBusMessage(reqStr)
            {
                ContentType = Constants.ApplicationJson
            };
            message.ApplicationProperties.Add(nameof(req.EventType), req.EventTypeEnum.ToString());

            return message;
        }
    }
}
