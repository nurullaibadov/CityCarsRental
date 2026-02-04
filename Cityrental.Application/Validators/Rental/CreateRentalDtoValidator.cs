using Cityrental.Application.DTOs.Rental;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Validators.Rental
{
    public class CreateRentalDtoValidator : AbstractValidator<CreateRentalDto>
    {
        public CreateRentalDtoValidator()
        {
            RuleFor(x => x.CarId)
                .NotEmpty().WithMessage("Car is required");

            RuleFor(x => x.PickupLocationId)
                .NotEmpty().WithMessage("Pickup location is required");

            RuleFor(x => x.ReturnLocationId)
                .NotEmpty().WithMessage("Return location is required");

            RuleFor(x => x.PickupDate)
                .NotEmpty().WithMessage("Pickup date is required")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Pickup date cannot be in the past");

            RuleFor(x => x.ReturnDate)
                .NotEmpty().WithMessage("Return date is required")
                .GreaterThan(x => x.PickupDate).WithMessage("Return date must be after pickup date");
        }
    }
}
