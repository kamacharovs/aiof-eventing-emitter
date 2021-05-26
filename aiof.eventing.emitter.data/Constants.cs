using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace aiof.eventing.emitter.data
{
    public static class Constants
    {
        public static JsonSerializerSettings JsonSettings =>
            new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
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
