using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IDomainEventHandler<in TEvent>
    {
        Task HandleAsync(TEvent domainEvent);
    }
}
