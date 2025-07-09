using Xunit;
using FluentValidation.TestHelper;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Validators;

namespace Portifolio.Teste.Application
{
    public class UnitTestUpdateProductDtoValidator
    {
        private readonly UpdateProductDtoValidator _validator;

        public UnitTestUpdateProductDtoValidator()
        {
            _validator = new UpdateProductDtoValidator();
        }

        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new UpdateProductDto
            (
                Name : "Produto Atualizado",
                Description : "Descrição atualizada",
                Brand : "Marca",
                Model : "Modelo",
                Year : 2024,
                Color : "Preto",
                Mileage : 500,
                CategoryId : 1
            );

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_Validation_For_Empty_Name()
        {
            var dto = new UpdateProductDto
            (
                Name : "",
                Description: "Descrição",
                Brand: "Marca",
                Model: "Modelo",
                Year: 2024,
                Color: "Preto",
                Mileage: 500,
                CategoryId: 1
            );

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Fail_Validation_For_Invalid_Year()
        {
            var dto = new UpdateProductDto
            (
                Name : "Produto",
                Description : "Descrição",
                Brand : "Marca",
                Model : "Modelo",
                Year : 1800,
                Color : "Preto",
                Mileage : 500,
                CategoryId : 1

            );

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Year);
        }
    }
}
