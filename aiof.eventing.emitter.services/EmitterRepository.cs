using System;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EmitterRepository : IEmitterRepository
    {
        public readonly ILogger<EmitterRepository> _logger;

        public EmitterRepository(
            ILogger<EmitterRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> EmitAsync(EventRequest req)
        {
            _logger.LogInformation("Event={EventType} triggered. Request={EventRequest}",
                req.EventTypeEnum.ToString(),
                req);

            return JsonSerializer.Serialize(req, Constants.JsonOptions);
        }
    }
}
