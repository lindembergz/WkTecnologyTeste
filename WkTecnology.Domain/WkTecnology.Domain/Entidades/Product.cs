using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using Portifolio.Core;
using Portifolio.Dominio.ValueObjects;

namespace Portifolio.Dominio.Entidades
{
    public class Product : BaseEntity
    {
        public ProductName Name { get; private set; } // Nome
        public string Description { get; private set; } // Descrição
        public string Brand { get; private set; } // Marca
        public string Model { get; private set; }  // Modelo
        public int Year { get; private set; } // Ano
        public string Color { get; private set; }   // Cor
        public int Mileage { get; private set; } // Quilometragem
        public bool IsActive { get; private set; } // Indica se o produto está ativo
        public int CategoryId { get; private set; } // ID da categoria do produto
        public Category Category { get; private set; } // Categoria do produto  

        private Product() { } 

        public Product(ProductName name, string description, string brand,
                      string model, int year, string color,
                      int mileage, int categoryId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description?.Trim() ?? string.Empty;
            Brand = brand?.Trim() ?? throw new ArgumentNullException(nameof(brand));
            Model = model?.Trim() ?? throw new ArgumentNullException(nameof(model));
            Year = ValidateYear(year);
            Color = color?.Trim() ?? throw new ArgumentNullException(nameof(color));
            Mileage = ValidateMileage(mileage);
            CategoryId = categoryId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateBasicInfo(ProductName name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateVehicleDetails(string brand, string model, int year,
                                       string color, int mileage)
        {
            Brand = brand?.Trim() ?? throw new ArgumentNullException(nameof(brand));
            Model = model?.Trim() ?? throw new ArgumentNullException(nameof(model));
            Year = ValidateYear(year);
            Color = color?.Trim() ?? throw new ArgumentNullException(nameof(color));
            Mileage = ValidateMileage(mileage);
            UpdatedAt = DateTime.UtcNow;
        }


        private static int ValidateYear(int year)
        {
            var currentYear = DateTime.Now.Year;
            if (year < 1900 || year > currentYear + 1)
                throw new ArgumentException($"Ano deve ter um valor entre 1900 and {currentYear + 1}");
            return year;
        }

        private static int ValidateMileage(int mileage)
        {
            if (mileage < 0)
                throw new ArgumentException("Quilometragem não pode ser negativa");
            return mileage;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }

}