using Aplication.Interfaces;
using Aplication.DTOs;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Aplication.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepository<Property> _propertyRepository;
        private readonly ILockRepository _lockRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        //private readonly ICurrentUserService _currentUser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationService(
            IRepository<Property> propertyRepository,
            ILockRepository lockRepository,
            IReservationRepository reservationRepository, 
            IHttpContextAccessor httpContextAccessor,
            //ICurrentUserService currentUser
            UserManager<ApplicationUser> userManager)
        {
            _lockRepository = lockRepository;
            _propertyRepository = propertyRepository;
            _reservationRepository = reservationRepository;
            _httpContextAccessor = httpContextAccessor;
            //_currentUser = currentUser;
            _userManager = userManager;
        }

        public async Task Add(int propertyId, ReservationCreateDTO dto)
        {
            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Unauthorized");

            var property = await _propertyRepository.GetValue(propertyId);

            if (property == null)
                throw new KeyNotFoundException("Property not found.");

            if(property.IdHost == userId)
            {
                throw new InvalidOperationException("You cannot reserve your own property.");
            }
            
            if(dto.StartDate > dto.EndDate)
            {
                throw new ArgumentException("Start date cannot be bigger than end date");
            }

            if (dto.GuestQuantity <= 0 || (dto.GuestQuantity > property.Capacity))
            {
                throw new ArgumentException($"The maximun guests must be {property.Capacity}");
            }

            if (!(await IsAvailable(propertyId, dto.StartDate, dto.EndDate)))
            {
                throw new InvalidOperationException("The property is not available in the selected dates");
            }

            var reservation = new Reservation
            {
                IdProperty = propertyId,
                IdGuest = userId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                GuestQuantity = dto.GuestQuantity,
                Status = ReservationStatus.Confirmed
            };

            await _reservationRepository.Add(reservation);
        }

        public async Task<IReservationVM> GetReservation(int id)
        {
            var reservation = await GetValue(id);

            if (reservation == null) throw new KeyNotFoundException("Reservation not found.");

            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();


            if (reservation.IdGuest == userId)
            {
                return await GuestReservation(reservation);
            }

            var hostId = (await _propertyRepository.GetValue(id)).IdHost;

            if (userId == hostId)
            {
                return await HostReservation(reservation);
            }
            
            throw new UnauthorizedAccessException("You dont't have permission to see this reservation");
        }

        private async Task<Reservation> GetValue(int id)
        {
            return await _reservationRepository.GetValue(id);
        }

        public async Task<IEnumerable<HostReservationVM>> GetAllReservationsVM(int idProperty)
        {
            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();

            var property = await _propertyRepository.GetValue(idProperty);


            if (property == null) throw new KeyNotFoundException("Property not found.");
            if (property.IdHost != userId) throw new UnauthorizedAccessException("You are not the owner of this property.");


            var reservations = (await _reservationRepository.GetAll()).Where(r => r.IdProperty == idProperty);

            var result = new List<HostReservationVM>();

            foreach (var reservation in reservations)
            {
                result.Add(await HostReservation(reservation));
            }

            return result;
        }

        /*public async Task<IEnumerable<HostReservationVM>> GetAllReservationsVM(int idProperty)
        {
            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var properties = (await _propertyRepository.GetAll()).Where(r => r.IdHost == userId);
            var reservations = GetAll().Where(r => r.IdProperty == idProperty);
            var reservationList = new List<HostReservationVM>();

            foreach (var reservation in reservations)
            {
                reservationList.Add(await HostReservation(reservation));
            }

            return reservationList;
        }*/

        private async Task<HostReservationVM> HostReservation(Reservation reservation)
        {
            string propertyTitle = (await _propertyRepository.GetValue(reservation.IdProperty)).Title;
            var guestName = await _userManager.FindByIdAsync(reservation.IdGuest);

            return new HostReservationVM
            {
                GuestName = guestName!.UserName!,
                PropertyTitle = propertyTitle,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                GuestQuantity = reservation.GuestQuantity,
                Status = reservation.Status
            };
        }

        private async Task<GuestReservationVM> GuestReservation(Reservation reservation)
        {
            string propertyTitle = (await _propertyRepository.GetValue(reservation.IdProperty)).Title;

            return new GuestReservationVM
            {
                Id = reservation.Id,
                PropertyTitle = propertyTitle,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                GuestQuantity = reservation.GuestQuantity,
                Status = reservation.Status
            };
        }

        public async Task CancelReservation(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();

            Reservation reservation = await GetValue(id);

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");

            if(reservation.Status != ReservationStatus.Confirmed)
                throw new InvalidOperationException("Cannot mark reservation as canceled.");

            reservation.Status = ReservationStatus.Canceled;

            await _reservationRepository.Update(reservation);
        }

        public async Task CompleteReservation(int id)
        {
            Reservation reservation = await GetValue(id);

            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException();

            if (reservation == null)
                throw new KeyNotFoundException("Reservation not found.");

            if (reservation.Status != ReservationStatus.Confirmed)
                throw new InvalidOperationException("Cannot mark reservation as completed.");

            if (reservation.EndDate > DateTime.Now)
                throw new InvalidOperationException("The reservation has not ended, can't mark as completed.");

            reservation.Status = ReservationStatus.Completed;
            await _reservationRepository.Update(reservation);
        }

        private async Task<IEnumerable<Reservation>> GetAll()
        {
            return await _reservationRepository.GetAll();
        }

        private async Task<bool> IsAvailable(int idProperty, DateTime startDate, DateTime endDate)
        {
            if (await _reservationRepository.OverlappingReservation(idProperty, startDate, endDate))
                return false;
            if (await _lockRepository.OverlappingBlock(idProperty, startDate, endDate))
                return false;

            return true;
        }

    }
}
