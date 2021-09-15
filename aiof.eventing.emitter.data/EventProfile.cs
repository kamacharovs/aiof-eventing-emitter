using System;
using System.Collections.Generic;
using System.Text;

using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;
using AutoMapper;

namespace aiof.eventing.emitter.data
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventRequest, ServiceBusMessage>()
                .ForMember(x => x.ContentType, o => o.MapFrom(s => Constants.ApplicationJson))
                .ForMember(x => x.Body, o => o.MapFrom(s => new BinaryData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(s, Constants.JsonSettings)))));
            CreateMap<EventConfig, ServiceBusMessage>()
                .ForMember(x => x.Subject, o => o.MapFrom(s => s.Subject));

            CreateMap<EventRequest, EventLog>()
                .ForMember(x => x.PartitionKey, o => o.MapFrom(s => s.EventType))
                .ForMember(x => x.RowKey, o => o.MapFrom(s => s.EventId))
                .ForMember(x => x.Raw, o => o.MapFrom(s => JsonConvert.SerializeObject(s, Constants.JsonSettings)));

            CreateMap<EventSource, EventLog>()
                .ForMember(x => x.SourceApi, o => o.MapFrom(s => s.Api))
                .ForMember(x => x.SourceIp, o => o.MapFrom(s => s.Ip));

            CreateMap<EventUser, EventLog>()
                .ForMember(x => x.UserId, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.UserPublicKey, o => o.MapFrom(s => s.PublicKey));

            CreateMap<EventEntity, EventLog>()
                .ForMember(x => x.EntityId, o => o.MapFrom(s => s.Id))
                .ForMember(x => x.EntityType, o => o.MapFrom(s => s.Type))
                .ForMember(x => x.EntityPayload, o => o.MapFrom(s => JsonConvert.SerializeObject(s.Payload, Constants.JsonSettings)));
        }
    }
}
