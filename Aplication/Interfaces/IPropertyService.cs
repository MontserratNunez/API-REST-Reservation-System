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
        IEnumerable<PropertyViewVM> GetAll();

        PropertyViewVM GetProperty(int id);

        void Add(PropertyDTO entity);

        void Update(int id, PropertyUpdateDTO entity);

        void Delete(int id);
    }
}
