using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkTecnology.Core;
using Portifolio.Dominio.ValueObjects;

namespace Portifolio.Dominio.Entidades
{
    public class Category : BaseEntity
    {
        public CategoryName Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        public int? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }

        private readonly List<Category> _subCategories = new();
        public IReadOnlyList<Category> SubCategories => _subCategories.AsReadOnly();

        private readonly List<Product> _products = new();
        public IReadOnlyList<Product> Products => _products.AsReadOnly();

        private Category() { } 

        public Category(CategoryName name, string description, int? parentCategoryId = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description?.Trim() ?? string.Empty;
            ParentCategoryId = parentCategoryId;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(CategoryName name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
