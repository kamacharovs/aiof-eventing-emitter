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

        public async Task<string> EmitAsync(EventRequest req)
        {
            await _logRepo.LogAsync(req);

            var config = await _configRepo.GetConfigAsync(req.EventTypeEnum.ToString());

            if (config is null)
                return null;

            return JsonSerializer.Serialize(req, Constants.JsonOptions);
        }
    }
}
