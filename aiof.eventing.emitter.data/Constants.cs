using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace aiof.eventing.emitter.data
{
    public static class Constants
    {
        public static JsonSerializerOptions JsonOptions =>
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                IgnoreNullValues = true
            };
    }

    public static class Keys
    {
        public const string StorageConnectionString = nameof(StorageConnectionString);
        public const string ServiceBusConnection = nameof(ServiceBusConnection);
        public const string EmitterTopicName = nameof(EmitterTopicName);
        public const string EmitterConfigTableName = nameof(EmitterConfigTableName);
        public const string EmitterLogTableName = nameof(EmitterLogTableName);
    }

    public enum EventType
    {
        NoMatch,
        AssetAdded
    }
}
