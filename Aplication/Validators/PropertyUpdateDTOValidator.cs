using Aplication.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Validators
{
    public class PropertyUpdateDTOValidator : AbstractValidator<PropertyUpdateDTO>
    {
        public PropertyUpdateDTOValidator()
        {
            RuleFor(x => x.Capacity)
                .NotEmpty()
                .WithMessage("Capacity is required")
                .GreaterThan(0)
                .WithMessage("Capacity must be greater than zero");
            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price is required")
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero");
        }
    }
}
