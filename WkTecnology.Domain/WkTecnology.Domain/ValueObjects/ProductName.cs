using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portifolio.Dominio.Base;

namespace Portifolio.Dominio.ValueObjects
{
    // Assuming the ValueObject base class is missing, we need to define it.
    // If it exists elsewhere in your project, ensure the correct namespace is imported.
    public class ProductName : ValueObject
    {
        public string Value { get; }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Product name cannot be empty");

            if (value.Length > 200)
                throw new ArgumentException("Product name cannot exceed 200 characters");

            Value = value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(ProductName productName) => productName.Value;
        public static implicit operator ProductName(string value) => new(value);
    }
}
