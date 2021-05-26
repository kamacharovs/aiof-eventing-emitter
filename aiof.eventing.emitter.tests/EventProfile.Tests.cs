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

        [Theory]
        [MemberData(nameof(Helper.EventRequests), MemberType = typeof(Helper))]
        public void EventRequest_To_EventLog_IsSuccessful(EventRequest eventRequest)
        {
            Assert.NotNull(eventRequest.EventId);
            Assert.NotNull(eventRequest.EventType);
            Assert.NotNull(eventRequest.Source);
            Assert.NotNull(eventRequest.User);
            Assert.NotNull(eventRequest.Entity);

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

        [Theory]
        [MemberData(nameof(Helper.EventSources), MemberType = typeof(Helper))]
        public void EventSource_To_EventLog_IsSuccessful(EventSource eventSource)
        {
            Assert.NotNull(eventSource);
            Assert.NotNull(eventSource.Api);
            Assert.NotNull(eventSource.Ip);

            var eventLog = _mapper.Map<EventLog>(eventSource);

            Assert.NotNull(eventLog);
            Assert.NotNull(eventLog.SourceApi);
            Assert.NotNull(eventLog.SourceIp);
        }

        [Theory]
        [MemberData(nameof(Helper.EventUsers), MemberType = typeof(Helper))]
        public void EventUser_To_EventLog_IsSuccessful(EventUser eventUser)
        {
            Assert.NotNull(eventUser);
            Assert.NotNull(eventUser.Id);
            Assert.NotNull(eventUser.PublicKey);

            var eventLog = _mapper.Map<EventLog>(eventUser);

            Assert.NotNull(eventLog);
            Assert.NotNull(eventLog.UserId);
            Assert.NotNull(eventLog.UserPublicKey);
        }

        [Theory]
        [MemberData(nameof(Helper.EventEntities), MemberType = typeof(Helper))]
        public void EventEntity_To_EventLog_IsSuccessful(EventEntity eventEntity)
        {
            Assert.NotNull(eventEntity);
            Assert.NotNull(eventEntity.Id);
            Assert.NotNull(eventEntity.Type);
            Assert.NotNull(eventEntity.Payload);

            var eventLog = _mapper.Map<EventLog>(eventEntity);

            Assert.NotNull(eventLog);
            Assert.NotNull(eventLog.EntityId);
            Assert.NotNull(eventLog.EntityType);
            Assert.NotNull(eventLog.EntityPayload);
        }
    }
}
