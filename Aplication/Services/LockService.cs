using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Services
{
    public class LockService : ILockService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepository<Property> _propertyRepository;
        private readonly ILockRepository _lockRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LockService(
            IRepository<Property> propertyRepository,
            ILockRepository lockRepository,
            IReservationRepository reservationRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _lockRepository = lockRepository;
            _propertyRepository = propertyRepository;
            _reservationRepository = reservationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Add(int idProperty, LockDTO dto)
        {
            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedDomainException("Unauthorized");

            var property = await _propertyRepository.GetValue(idProperty);

            if (property == null)
                throw new ResourceNotFoundException("Property not found.");

            if (property.IdHost != userId)
            {
                throw new UnauthorizedDomainException("You cannot lock this property.");
            }

            //FluentValidation

            if (dto.StartDate > dto.EndDate)
            {
                throw new ArgumentException("Start date cannot be bigger than end date");
            }

            if (!(await IsAvailable(idProperty, dto.StartDate, dto.EndDate)))
            {
                throw new BusinessRuleException("The property already has reservations or have been locked in the selected dates");
            }

            var block = new Lock
            {
                IdProperty = idProperty,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _lockRepository.Add(block);
        }

        public async Task Delete(int id)
        {
            try
            {
                await _lockRepository.Delete(id);
            }
            catch (Exception)
            {
                throw new ResourceNotFoundException("There is no lock.");
            }
        }

        public async Task<IEnumerable<LockVM>> GetPropertyLocks(int idProperty)
        {
            //Change to ICurrentUser
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedDomainException("Unauthorized");

            var property = await _propertyRepository.GetValue(idProperty);


            if (property == null) throw new ResourceNotFoundException("Property not found.");
            if (property.IdHost != userId) throw new UnauthorizedDomainException("You are not the owner of this property.");


            var blocks = (await _lockRepository.GetAll()).Where(r => r.IdProperty == idProperty);

            var result = new List<LockVM>();

            foreach (var block in blocks)
            {
                result.Add(GetLockVM(block));
            }

            return result;
        }

        private LockVM GetLockVM(Lock block)
        {
            return new LockVM
            {
                Id = block.Id,
                StartDate = block.StartDate,
                EndDate = block.EndDate
            };
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
