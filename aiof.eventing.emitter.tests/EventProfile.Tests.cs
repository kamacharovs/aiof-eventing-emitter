using System;

using Xunit;
using AutoMapper;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.tests
{
    public class EventProfileTests
    {
        public readonly IMapper _mapper;

        public EventProfileTests()
        {
            _mapper = Helper.GetRequiredService<IMapper>() ?? throw new ArgumentNullException(nameof(AutoMapper));
        }

        [Fact]
        public void EventRequest_To_EventLog_IsSuccessful()
        {
            var eventRequest = Helper.GetRandomEventRequest();

            var eventLog = _mapper.Map<EventLog>(eventRequest);

            Assert.NotNull(eventLog);
            Assert.NotNull(eventLog.PartitionKey);
            Assert.NotNull(eventLog.RowKey);
            Assert.NotNull(eventLog.SourceApi);
            Assert.NotNull(eventLog.SourceIp);
            Assert.NotNull(eventLog.UserId);
            Assert.NotNull(eventLog.UserPublicKey);
            Assert.NotNull(eventLog.EntityId);
            Assert.NotNull(eventLog.EntityType);
            Assert.NotNull(eventLog.EntityPayload);
            Assert.NotNull(eventLog.Raw);
        }
    }
}
