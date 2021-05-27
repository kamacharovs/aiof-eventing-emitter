using System;

using Newtonsoft.Json;

namespace aiof.eventing.emitter.data
{
    public class EventRequest
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventType { get; set; }
        public EventSource Source { get; set; }
        public EventUser User { get; set; }
        public EventEntity Entity { get; set; }

        [JsonIgnore]
        public EventType EventTypeEnum
        {
            get
            {
                Enum.TryParse<EventType>(EventType, out var EventTypeEnum);

                return EventTypeEnum;
            }
        }
    }

    public class EventSource
    {
        public string Api { get; set; }
        public string Ip { get; set; }
    }

    public class EventUser
    {
        public int? Id { get; set; }
        public Guid? PublicKey { get; set; }
    }

    public class EventEntity
    {
        public int? Id { get; set; }
        public string Type { get; set; }
        public object Payload { get; set; }
    }
}
