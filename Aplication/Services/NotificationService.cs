using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(
            IRepository<Notification> repository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Create(string userId, string title, string message)
        {
            var notification = new Notification
            {
                IdUser = userId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await _repository.Add(notification);
        }

        public async Task<IEnumerable<NotificationVM>> GetAll()
        {
            var userId = _httpContextAccessor.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = (await _repository.GetAll())
                .Where(n => n.IdUser == userId)
                .OrderByDescending(n => n.CreatedAt);

            return notifications.Select(n => new NotificationVM
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            });
        }

        public async Task<NotificationVM> Get(int id)
        {
            var userId = _httpContextAccessor.HttpContext!
                .User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notification = await _repository.GetValue(id);

            if (notification == null || notification.IdUser != userId)
                throw new UnauthorizedDomainException("Access denied");

            return new NotificationVM
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }

        public async Task MarkAsRead(int id)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedDomainException("Unauthorized");

            var notification = await _repository.GetValue(id);


            if (notification.IdUser != userId)
                throw new UnauthorizedDomainException("You are not allowed to modify this notification.");

            if (notification.IsRead)
                throw new BusinessRuleException("Notification is already marked as read.");

            notification.IsRead = true;
            await _repository.Update(notification);
        }
    }
}
