using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.DTOs;
using FluentValidation;

namespace Aplication.Validators
{
    public class ReservationCreateDTOValidator : AbstractValidator<ReservationCreateDTO>
    {
        public ReservationCreateDTOValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be greater than start date");

            RuleFor(x => x.GuestQuantity)
                .GreaterThan(0)
                .WithMessage("Guest quantity must be greater than zero");
        }
    }
}
