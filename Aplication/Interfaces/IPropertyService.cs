using Aplication.DTOs;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyViewVM>> GetAll();
        Task<IEnumerable<PropertyViewVM>> Search(PropertySearchDTO filters);

        Task<PropertyViewVM> GetProperty(int id);

        Task Add(PropertyDTO entity);

        Task Update(int id, PropertyUpdateDTO entity);

        Task Delete(int id);

        Task<IEnumerable<PropertyViewVM>> GetHostProperties();
    }
}
