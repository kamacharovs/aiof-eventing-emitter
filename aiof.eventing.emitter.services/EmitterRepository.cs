using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;
using AutoMapper;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EmitterRepository : IEmitterRepository
    {
        private readonly ILogger<EmitterRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IEventConfigRepository _configRepo;
        private readonly IEventLogRepository _logRepo;

        public EmitterRepository(
            ILogger<EmitterRepository> logger,
            IMapper mapper,
            IEventConfigRepository configRepo,
            IEventLogRepository logRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configRepo = configRepo ?? throw new ArgumentNullException(nameof(configRepo));
            _logRepo = logRepo ?? throw new ArgumentNullException(nameof(logRepo));
        }

        public async Task<ServiceBusMessage> EmitAsync(EventRequest req)
        {
            await _logRepo.LogAsync(req);

            var config = await _configRepo.GetConfigAsync(req.EventType);

            if (config is null)
                return null;

            var message = _mapper.MergeInto<ServiceBusMessage>(req, config);

            message.ApplicationProperties.Add(nameof(EventRequest.EventId), req.EventId);
            message.ApplicationProperties.Add(nameof(EventRequest.EventType), req.EventType);

            return message;
        }
    }
}
