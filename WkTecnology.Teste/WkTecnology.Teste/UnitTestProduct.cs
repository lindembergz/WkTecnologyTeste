using Xunit;
using Portifolio.Dominio.Entidades;
using Portifolio.Dominio.ValueObjects;

namespace WkTecnology.Teste
{
    public class UnitTestProduct
    {
        [Fact]
        public void CreateProduct_WithValidValues_SetsPropertiesCorrectly()
        {
            
            var productName = new ProductName("Test Product");
            string description = "Test Description";
            string brand = "Test Brand";
            string model = "Test Model";
            int year = 2025;
            string color = "Red";
            int mileage = 100;
            int categoryId = 1;
           
            var product = new Product(productName, description, brand, model, year, color, mileage, categoryId);
          
            Assert.Equal("Test Product", product.Name.Value);
            Assert.Equal("Test Description", product.Description);
            Assert.Equal("Test Brand", product.Brand);
            Assert.Equal("Test Model", product.Model);
            Assert.Equal(2025, product.Year);
            Assert.Equal("Red", product.Color);
            Assert.Equal(100, product.Mileage);
            Assert.Equal(1, product.CategoryId);
            Assert.True(product.IsActive); 
        }

        [Fact]
        public void UpdateBasicInfo_ShouldChangeNameAndDescription()
        {            
            var product = new Product(new ProductName("Old Name"), "Old Description", "Brand", "Model", 2021, "Blue", 5000, 1);
           
            product.UpdateBasicInfo(new ProductName("New Name"), "New Description");
            
            Assert.Equal("New Name", product.Name.Value);
            Assert.Equal("New Description", product.Description);
        }

        [Fact]
        public void ActivateAndDeactivate_ShouldToggleIsActive()
        {
        
            var product = new Product(new ProductName("Test Product"), "Description", "Brand", "Model", 2021, "Blue", 5000, 1);
       
            Assert.True(product.IsActive);
   
            product.Deactivate();
                   
            Assert.False(product.IsActive);
     
            product.Activate();
       
            Assert.True(product.IsActive);
        }
    }
}
