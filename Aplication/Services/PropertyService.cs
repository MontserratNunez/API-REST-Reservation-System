using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
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

        private readonly IHttpContextAccessor _httpContextAccessor;


        public PropertyService(IRepository<Property> repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Add(PropertyDTO dto)
        {

            var userId = _httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var property = new Property
            {
                IdHost = userId,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                Price = dto.Price,
                Capacity = dto.Capacity,
            };


            _repository.Add(property);
        }

        public void Delete(int id)
        {
            var original = GetValue(id);

            if (original == null)
                throw new KeyNotFoundException("Property not found.");


            var userId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (original.IdHost != userId)
            {
                throw new UnauthorizedAccessException("You are not allowed to modify this property.");
            }


            _repository.Delete(id);
        }

        public IEnumerable<PropertyViewVM> GetAll()
        {
            var properties = _repository.GetAll();

            return properties.Select(p => new PropertyViewVM
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Location = p.Location,
                Price = p.Price,
                Capacity = p.Capacity
            });

        }

        public PropertyViewVM GetProperty(int id)
        {
            var property = GetValue(id);


            if (property == null)
            {
                throw new KeyNotFoundException("Property not found.");
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

        private Property GetValue(int id)
        {
            return _repository.GetValue(id);
        }

        public void Update(int id, PropertyUpdateDTO dto)
        {
            var original = GetValue(id);

            if (original == null)
            {
                throw new KeyNotFoundException("Property not found.");
            }

            var userId = _httpContextAccessor.HttpContext.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (original.IdHost != userId)
            {
                throw new UnauthorizedAccessException("You are not allowed to modify this property.");
            }

            original.Title = !string.IsNullOrEmpty(dto.Title) ? dto.Title : original.Title;
            original.Description = !string.IsNullOrEmpty(dto.Description) ?  dto.Description : original.Description;
            original.Location = !string.IsNullOrEmpty(dto.Location) ?  dto.Location : original.Location;
            original.Price = dto.Price ?? original.Price;
            original.Capacity = dto.Capacity ?? original.Capacity;

            _repository.Update(original);
        }
    }
}
