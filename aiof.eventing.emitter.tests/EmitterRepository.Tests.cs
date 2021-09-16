using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Newtonsoft.Json;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.tests
{
    public class EmitterRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.EventTypes), MemberType = typeof(Helper))]
        public async Task EmitAsync_IsSuccessful(string eventType)
        {
            var repo = Helper.GetEmitterRepository(eventType);
            var eventRequest = Helper.GetRandomEventRequest();

            eventRequest.EventType = eventType;

            var eventMessage = await repo.EmitAsync(eventRequest);

            Assert.NotNull(eventMessage);

            var eventMessageRequest = JsonConvert.DeserializeObject<EventRequest>(Encoding.UTF8.GetString(eventMessage.Body));

            Assert.NotNull(eventMessageRequest);
            Assert.NotNull(eventMessageRequest.EventId);
            Assert.NotNull(eventMessageRequest.EventType);
            Assert.NotNull(eventMessageRequest.Source);
            Assert.NotNull(eventMessageRequest.User);
            Assert.NotNull(eventMessageRequest.Entity);
            Assert.Equal(eventType, eventMessageRequest.EventType);
        }
    }
}
