using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using AutoMapper;

using aiof.eventing.emitter.data;
using aiof.eventing.emitter.services;

namespace aiof.eventing.emitter.tests
{
    public class EventLogRepositoryTests
    {
        [Theory]
        [MemberData(nameof(Helper.EventTypes), MemberType = typeof(Helper))]
        public async Task LogAsync_IsSuccessful(string eventType)
        {
            var repo = Helper.GetEventLogRepository(eventType);
            var eventRequest = Helper.GetRandomEventRequest();

            eventRequest.EventType = eventType;

            await repo.LogAsync(eventRequest);
        }

        [Theory]
        [MemberData(nameof(Helper.EventTypes), MemberType = typeof(Helper))]
        public async Task InsertAsync_IsSuccessful(string eventType)
        {
            var repo = Helper.GetEventLogRepository(eventType);
            var eventRequest = Helper.GetRandomEventRequest();

            eventRequest.EventType = eventType;

            var mapper = Helper.GetRequiredService<IMapper>();
            
            await repo.InsertAsync(mapper.Map<EventLog>(eventRequest));
        }
    }
}
