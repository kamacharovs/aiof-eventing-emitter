using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using AutoMapper;

namespace aiof.eventing.emitter.data
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventRequest, EventLog>()
                .ForMember(x => x.PartitionKey, o => o.MapFrom(s => s.EventTypeEnum.ToString()))
                .ForMember(x => x.RowKey, o => o.MapFrom(s => s.EventId))
                .ForMember(x => x.Raw, o => o.MapFrom(s => JsonSerializer.Serialize(s, Constants.JsonOptions)));

            CreateMap<EventSource, EventLog>()
                .ForMember(x => x.SourceApi, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Api)))
                .ForMember(x => x.SourceIp, o => o.Condition(s => !string.IsNullOrWhiteSpace(s.Ip)));

            CreateMap<EventUser, EventLog>()
                .ForMember(x => x.UserId, o => o.Condition(s => s.Id.HasValue))
                .ForMember(x => x.UserPublicKey, o => o.Condition(s => s.PublicKey.HasValue));
        }
    }
}
