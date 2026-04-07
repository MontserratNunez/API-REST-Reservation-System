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
    public class PropertyService : IPropertyService
    {
        private readonly IRepository<Property> _repository;
        private readonly IReservationRepository _reservationRepository;
        private readonly ILockRepository _lockRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public PropertyService(IRepository<Property> repository,
            ILockRepository lockRepository,
            IReservationRepository reservationRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _lockRepository = lockRepository;
            _reservationRepository = reservationRepository;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Add(PropertyDTO dto)
        {

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedDomainException("User not authenticated");
            }

            //FluentValidation
            var property = new Property
            {
                IdHost = userId,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Price = dto.Price,
                Capacity = dto.Capacity,
            };

            await _repository.Add(property);
        }

        public async Task Delete(int id)
        {
            var original = await GetValue(id);

            if (original == null)
                throw new ResourceNotFoundException("Property not found.");


            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (original.IdHost != userId)
            {
                throw new UnauthorizedDomainException("You are not allowed to modify this property.");
            }

            await _repository.Delete(id);
        }

        public async Task<IEnumerable<PropertyViewVM>> GetAll()
        {
            return (await _repository.GetAll()).Select(p => new PropertyViewVM
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Price = p.Price,
                Capacity = p.Capacity
            });

        }

        public async Task<IEnumerable<PropertyViewVM>> Search(PropertySearchDTO filters)
        {
            var query = (await _repository.GetAll()).AsQueryable();

            if (!string.IsNullOrEmpty(filters.Location))
            {
                query = query.Where(p => p.Location.ToLower().Contains(filters.Location.ToLower()));
            }

            if (filters.Capacity.HasValue)
            {
                query = query.Where(p => p.Capacity >= filters.Capacity.Value);
            }

            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            }

            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            if (filters.StartDate.HasValue && filters.EndDate.HasValue)
            {
                var reservedIds =await _reservationRepository.GetOverlappingPropertyIds(filters.StartDate.Value,filters.EndDate.Value);

                var blockedIds =await _lockRepository.GetOverlappingPropertyIds(filters.StartDate.Value,filters.EndDate.Value);

                var unavailableIds = reservedIds.Union(blockedIds).ToHashSet();

                query = query.Where(p => !unavailableIds.Contains(p.Id));
            }

            return query.Select(p => new PropertyViewVM
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Price = p.Price,
                Capacity = p.Capacity
            });
        }

        public async Task<PropertyViewVM> GetProperty(int id)
        {
            var property = await GetValue(id);

            if (property == null)
            {
                throw new ResourceNotFoundException("Property not found.");
            }

            return new PropertyViewVM
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                Location = property.Location,
                Price = property.Price,
                Capacity = property.Capacity
            };

        }

        private async Task<Property> GetValue(int id)
        {
            return await _repository.GetValue(id);
        }

        public async Task Update(int id, PropertyUpdateDTO dto)
        {
            var original = await GetValue(id);

            if (original == null)
            {
                throw new ResourceNotFoundException("Property not found.");
            }

            var userId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (original.IdHost != userId)
            {
                throw new UnauthorizedDomainException("You are not allowed to modify this property.");
            }

            original.Title = !string.IsNullOrEmpty(dto.Title) ? dto.Title : original.Title;
            original.Description = !string.IsNullOrEmpty(dto.Description) ?  dto.Description : original.Description;
            original.Location = !string.IsNullOrEmpty(dto.Location) ?  dto.Location : original.Location;
            original.Price = dto.Price ?? original.Price;
            original.Capacity = dto.Capacity ?? original.Capacity;

            await _repository.Update(original);
        }
    }
}
