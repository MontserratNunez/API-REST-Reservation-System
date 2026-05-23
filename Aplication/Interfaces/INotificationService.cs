using Aplication.DTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface INotificationService
    {
        Task Create(string userId, string title, string message);

        Task<IEnumerable<NotificationVM>> GetAll();
        Task<NotificationVM> Get(int id);
        Task MarkAsRead(int id);
    }

}
