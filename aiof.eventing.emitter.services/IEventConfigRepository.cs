using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public interface IEventConfigRepository
    {
        Task<EventConfig> GetConfigAsync(string eventType);
    }
}
