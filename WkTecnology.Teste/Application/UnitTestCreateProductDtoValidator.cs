using Xunit;
using FluentValidation.TestHelper;
using Portifolio.Aplicacao.DTOs;
using Portifolio.Aplicacao.Validators;

namespace WkTecnology.Teste.Application
{
    public class UnitTestCreateProductDtoValidator
    {
        private readonly CreateProductDtoValidator _validator;

        public UnitTestCreateProductDtoValidator()
        {
            _validator = new CreateProductDtoValidator();
        }

        [Fact]
        public void Should_Pass_Validation_For_Valid_Dto()
        {
            var dto = new CreateProductDto
            (
                Name: "Produto Teste",
                Description: "Descrição válida",
                Brand: "Marca",
                Model: "Modelo",
                Year: 2024,
                Color: "Azul",
                Mileage: 1000,
                CategoryId: 1,
                IsActive: true
            );

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_Validation_For_Empty_Name()
        {
            var dto = new CreateProductDto
            (
                Name: "",
                Description: "Descrição válida",
                Brand: "Marca",
                Model: "Modelo",
                Year: 2024,
                Color: "Azul",
                Mileage: 1000,
                CategoryId: 1,
                IsActive: true
            );

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Fail_Validation_For_Invalid_Year()
        {
            var dto = new CreateProductDto
            (
                Name: "Produto Teste",
                Description: "Descrição válida",
                Brand: "Marca",
                Model: "Modelo",
                Year: 1800, // Supondo que o ano mínimo seja maior
                Color: "Azul",
                Mileage: 1000,
                CategoryId: 1,
                IsActive: true
            );

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Year);
        }
    }
}
