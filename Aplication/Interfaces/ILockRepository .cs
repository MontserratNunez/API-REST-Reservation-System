using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface ILockRepository : IRepository<Lock>
    {
        Task<bool> OverlappingBlock(int propertyId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<int>> GetOverlappingPropertyIds(DateTime startDate,DateTime endDate);

    }
}
