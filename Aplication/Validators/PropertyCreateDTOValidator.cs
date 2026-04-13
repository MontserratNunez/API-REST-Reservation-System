using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTOs;
using FluentValidation;

namespace Aplication.Validators
{
    public class PropertyCreateDTOValidator : AbstractValidator<PropertyDTO>
    {
        public PropertyCreateDTOValidator()
        {
            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity must be greater than zero");
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero");
        }
    }
}
