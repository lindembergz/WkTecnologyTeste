using Xunit;
using Portifolio.Dominio.Entidades;
using Portifolio.Dominio.ValueObjects;

namespace WkTecnology.Teste.Domain
{
    public class UnitTestCategory
    {
        [Fact]
        public void CreateCategory_WithValidValues_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var category = new Category(CategoryName.Create("Electronics"), "Devices and gadgets");

            // Assert
            Assert.Equal("Electronics", category.Name.Value); // Corrigido para acessar o valor de CategoryName
            Assert.Equal("Devices and gadgets", category.Description);
            Assert.True(category.IsActive); // Assumindo que a categoria é ativa por padrão
        }

        [Fact]
        public void UpdateCategory_ShouldChangeNameAndDescription()
        {
            // Arrange
            var category = new Category(CategoryName.Create("Old Name"), "Old Description");

            // Act
            category.UpdateDetails(CategoryName.Create("New Name"), "New Description");

            // Assert
            Assert.Equal("New Name", category.Name.Value); // Corrigido para acessar o valor de CategoryName
            Assert.Equal("New Description", category.Description);
        }

        [Fact]
        public void ActivateAndDeactivate_Category_ShouldToggleIsActive()
        {
            // Arrange
            var category = new Category(CategoryName.Create("Test Category"), "Test Description");

            // Assert initial state (assumindo que é ativa)
            Assert.True(category.IsActive);

            // Act - Desativar
            category.Deactivate();
            Assert.False(category.IsActive);

            // Act - Ativar
            category.Activate();
            Assert.True(category.IsActive);
        }
    }
}
