using Cityrental.Application.DTOs.Car;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cityrental.Application.Validators.Car
{
    public class CreateCarDtoValidator : AbstractValidator<CreateCarDto>
    {
        public CreateCarDtoValidator()
        {
            RuleFor(x => x.BrandId)
                .NotEmpty().WithMessage("Brand is required");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required")
                .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");

            RuleFor(x => x.Year)
                .GreaterThanOrEqualTo(2000).WithMessage("Year must be 2000 or later")
                .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Invalid year");

            RuleFor(x => x.LicensePlate)
                .NotEmpty().WithMessage("License plate is required");

            RuleFor(x => x.DailyRate)
                .GreaterThan(0).WithMessage("Daily rate must be greater than 0");

            RuleFor(x => x.Seats)
                .GreaterThan(0).WithMessage("Seats must be at least 1")
                .LessThanOrEqualTo(12).WithMessage("Seats cannot exceed 12");

            RuleFor(x => x.Doors)
                .GreaterThan(0).WithMessage("Doors must be at least 1")
                .LessThanOrEqualTo(6).WithMessage("Doors cannot exceed 6");
        }
    }
}
