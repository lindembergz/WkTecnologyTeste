using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portifolio.Core;

namespace Portifolio.Dominio.ValueObjects
{
    public class ProductName : ValueObject
    {
        public string Value { get; }

        public ProductName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Nome do produto não pode ser vazio");

            if (value.Length > 200)
                throw new ArgumentException("Nome do produto não pode exceder 200 caracteres");

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
