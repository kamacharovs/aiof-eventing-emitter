using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using aiof.eventing.emitter.data;

namespace aiof.eventing.emitter.services
{
    public interface IEventLogRepository
    {
        Task LogAsync(EventRequest req);
        Task InsertAsync(EventLog log);

    }
}
