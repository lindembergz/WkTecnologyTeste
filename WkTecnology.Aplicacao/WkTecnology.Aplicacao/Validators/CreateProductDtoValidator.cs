using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Portifolio.Aplicacao.DTOs;

namespace Portifolio.Aplicacao.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is required")
                .MaximumLength(100).WithMessage("Brand cannot exceed 100 characters");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required")
                .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");

            RuleFor(x => x.Year)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .WithMessage($"Year must be between 1900 and {DateTime.Now.Year + 1}");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .MaximumLength(50).WithMessage("Color cannot exceed 50 characters");

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Valid category is required");
        }
    }
}
