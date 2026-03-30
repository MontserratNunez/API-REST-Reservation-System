using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IRepository<Property> _repository;

        public PropertyService(IRepository<Property> repository)
        {
            _repository = repository;
        }
        public void Add(PropertyDTO dto)
        {
            var property = new Property
            {
                IdHost = dto.IdHost,
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
            var propiedad = GetValue(dto.Id);

            if (propiedad == null)
            {
                throw new KeyNotFoundException("Property not found.");
            }

            //Agregar autenticacion con token
            /*if (dto.IdHost != original.IdHost)
            {
                throw new UnauthorizedAccessException("You are not allowed to update this property.");
            }*/

            _repository.Delete(id);
        }

        public IEnumerable<Property> GetAll()
        {
            return _repository.GetAll();
        }

        public Property GetValue(int id)
        {
            return _repository.GetValue(id);
        }

        public void Update(PropertyUpdateDTO dto)
        {
            var original = GetValue(dto.Id);

            if (original == null)
            {
                throw new KeyNotFoundException("Property not found.");
            }

            //Agregar autenticacion con token
            if (dto.IdHost != original.IdHost)
            { 
                throw new UnauthorizedAccessException("You are not allowed to update this property."); 
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
