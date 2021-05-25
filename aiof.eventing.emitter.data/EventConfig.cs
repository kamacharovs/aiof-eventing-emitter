using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Cosmos.Table;

namespace aiof.eventing.emitter.data
{
    public class EventConfig : TableEntity
    {
        public EventConfig() { }

        public EventConfig(string eventType)
        {
            PartitionKey = eventType;
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
