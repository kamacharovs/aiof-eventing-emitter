using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;

using AutoMapper;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EventConfigRepository : IEventConfigRepository
    {
        private readonly ILogger<EventConfigRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly CloudTableClient _client;

        public EventConfigRepository(
            ILogger<EventConfigRepository> logger,
            IMapper mapper,
            IConfiguration config,
            CloudTableClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<EventConfig> GetConfigAsync(string eventType)
        {
            var table = _client.GetTableReference(_config[Keys.EmitterConfigTableName]);
            var eventTypeFilter = TableQuery.GenerateFilterCondition(nameof(EventConfig.PartitionKey), QueryComparisons.Equal, eventType);
            var query = new TableQuery<EventConfig>().Where(eventTypeFilter);

            try
            {
                return await Task.Run(() =>
                {
                    return table.ExecuteQuery(query)
                        .FirstOrDefault();
                });
            }
            catch (StorageException se)
            {
                _logger.LogError(se, $"{nameof(GetConfigAsync)} storage query exception. Message={se.Message}. Filter={eventTypeFilter}");
            }

            return null;
        }
    }
}
