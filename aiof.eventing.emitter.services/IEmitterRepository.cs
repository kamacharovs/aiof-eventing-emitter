using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public interface IEmitterRepository
    {
        Task<string> EmitAsync(EventRequest req);
    }
}
