using System;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EmitterRepository : IEmitterRepository
    {
        private readonly ILogger<EmitterRepository> _logger;
        private readonly IEventConfigRepository _configRepo;

        public EmitterRepository(
            ILogger<EmitterRepository> logger,
            IEventConfigRepository configRepo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configRepo = configRepo ?? throw new ArgumentNullException(nameof(configRepo));
        }

        public async Task<string> EmitAsync(EventRequest req)
        {
            _logger.LogInformation("Event={EventType} triggered. Request={EventRequest}",
                req.EventTypeEnum.ToString(),
                req);

            var config = await _configRepo.GetConfigAsync(req.EventTypeEnum.ToString());

            if (config is null)
                return null;

            return JsonSerializer.Serialize(req, Constants.JsonOptions);
        }
    }
}
