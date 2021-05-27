using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public interface IEmitterRepository
    {
        Task<ServiceBusMessage> EmitAsync(EventRequest req);
    }
}
