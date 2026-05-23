using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTOs;
using Domain.Entity;

namespace Aplication.Interfaces
{
    public interface ILockService
    {
        Task<IEnumerable<LockVM>> GetPropertyLocks(int idProperty);
        Task Add(int idProperty, LockDTO dto);
        Task Delete(int id);
    }
}
