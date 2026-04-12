using Aplication.DTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IReservationService
    {
        Task Add(int propertyId, ReservationCreateDTO dto);
        Task<IReservationVM> GetReservation(int id);
        Task CancelReservation(int id);
        Task CompleteReservation(int id);
        Task<IEnumerable<HostReservationVM>> GetAllPropertyReservationsVM(int idProperty);
        Task<IEnumerable<GuestReservationVM>> GetAllGuestReservationsVM();
        Task<IEnumerable<UnavailableDate>> GetUnavailableDates(int idProperty);
    }
}
