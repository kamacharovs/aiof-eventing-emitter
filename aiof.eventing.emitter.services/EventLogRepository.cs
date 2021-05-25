using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;

using AutoMapper;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly ILogger<EventLogRepository> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly CloudTableClient _client;

        public EventLogRepository(
            ILogger<EventLogRepository> logger,
            IMapper mapper,
            IConfiguration config,
            CloudTableClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task LogAsync(EventRequest req)
        {
            var eventLog = _mapper.Map<EventLog>(req);

            await InsertAsync(eventLog);
        }

        public async Task InsertAsync(EventLog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            var tableName = _config[Keys.EmitterLogTableName];
            var table = _client.GetTableReference(tableName);
            var insertOperation = TableOperation.Insert(log);

            try
            {
                await table.ExecuteAsync(insertOperation);
            }
            catch (StorageException se)
            {
                _logger.LogError(se, $"{nameof(InsertAsync)} storage insert exception. Message={se.Message}");
            }
        }
    }
}
