using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Azure.Cosmos.Table;

namespace aiof.eventing.emitter.data
{
    public class EventLog : TableEntity
    {
        public EventLog() { }

        public EventLog(string eventType, string eventId)
        {
            PartitionKey = eventType;
            RowKey = eventId;
        }

        public string SourceApi { get; set; }
        public string SourceIp { get; set; }
        public int? UserId { get; set; }
        public Guid? UserPublicKey { get; set; }
        public string Raw { get; set; }
    }
}
