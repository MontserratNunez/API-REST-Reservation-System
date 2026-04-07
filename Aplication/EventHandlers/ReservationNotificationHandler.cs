using Aplication.Interfaces;
using Domain.Entity;
using Domain.Events;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.EventHandlers
{
    public class ReservationNotificationHandler :
    IDomainEventHandler<ReservationCreatedEvent>,
    IDomainEventHandler<ReservationCanceledEvent>,
    IDomainEventHandler<ReservationCompletedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationNotificationHandler(
            INotificationService notificationService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task HandleAsync(ReservationCreatedEvent e)
        {
            var host = await _userManager.FindByIdAsync(e.Property.IdHost);

            await _notificationService.Create(
                e.Property.IdHost,
                "New reservation",
                $"A new reservation was made for {e.Property.Title}\nFrom {e.Reservation.StartDate} to {e.Reservation.EndDate}"
            );

            _ = Task.Run(() =>
                _emailService.SendEmail(
                    host.Email,
                    "New reservation",
                    $"A new reservation was made for {e.Property.Title}"
                ));
        }

        public async Task HandleAsync(ReservationCanceledEvent e)
        {
            var host = await _userManager.FindByIdAsync(e.Property.IdHost);
            var guest = await _userManager.FindByIdAsync(e.Reservation.IdGuest);

            string title = "Reservation canceled";
            string hostMessage = $"The guest has canceled the reservation for {e.Property.Title}\nFrom {e.Reservation.StartDate} to {e.Reservation.EndDate}";
            string guestMessage = $"Reservation for {e.Property.Title} canceled";

            await _notificationService.Create(e.Property.IdHost, title, hostMessage);
            await _notificationService.Create(e.Reservation.IdGuest, title, guestMessage);
            

            _ = Task.Run(() =>
            {
                _emailService.SendEmail(host!.Email!, title, hostMessage);
                _emailService.SendEmail(guest!.Email!, title, guestMessage);
            });
        }

        public async Task HandleAsync(ReservationCompletedEvent e)
        {
            var guest = await _userManager.FindByIdAsync(e.Reservation.IdGuest);

            string title = "Reservation completed";
            string message = $"The reservation for {e.Property.Title} for has been completed.";

            await _notificationService.Create(e.Reservation.IdGuest, title, message);

            _ = Task.Run(() =>
            {
                _emailService.SendEmail(guest.Email, title, message);
            });
        }
    }
}
