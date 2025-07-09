using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Portifolio.Aplicacao.DTOs;

namespace Portifolio.Aplicacao.Validators
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório")
                .MaximumLength(200).WithMessage("O nome do produto não pode exceder 200 caracteres");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres");

            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("A marca é obrigatória")
                .MaximumLength(100).WithMessage("A marca não pode exceder 100 caracteres");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("O modelo é obrigatório")
                .MaximumLength(100).WithMessage("O modelo não pode exceder 100 caracteres");

            RuleFor(x => x.Year)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .WithMessage($"O ano deve estar entre 1900 e {DateTime.Now.Year + 1}");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("A cor é obrigatória")
                .MaximumLength(50).WithMessage("A cor não pode exceder 50 caracteres");

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0).WithMessage("A quilometragem não pode ser negativa");

        }
    }
}
