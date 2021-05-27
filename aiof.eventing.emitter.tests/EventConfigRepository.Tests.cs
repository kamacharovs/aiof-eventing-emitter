using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.tests
{
    public class EventConfigRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.EventTypes), MemberType = typeof(Helper))]
        public async Task GetConfigAsync_IsSuccessful(string eventType)
        {
            var repo = Helper.GetEventConfigRepository(eventType);

            var config = await repo.GetConfigAsync(eventType);

            Assert.NotNull(config);
            Assert.NotNull(config.PartitionKey);
            Assert.NotNull(config.RowKey);
            Assert.Equal(eventType, config.PartitionKey);
        }

        [Theory]
        [MemberData(nameof(Helper.EventTypes), MemberType = typeof(Helper))]
        public async Task GetConfigAsync_NotFound_Returns_Null(string eventType)
        {
            var repo = Helper.GetEventConfigRepository(eventType);

            var config = await repo.GetConfigAsync($"{eventType}NotFound");

            Assert.Null(config);
        }
    }
}
