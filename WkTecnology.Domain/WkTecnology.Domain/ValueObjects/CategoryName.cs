using Portifolio.Dominio.Base;

namespace Portifolio.Dominio.ValueObjects
{
    public abstract class CategoryName : ValueObject
    {
        public string Value { get; }

        public CategoryName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome da categoria não pode ser vazio.", nameof(value));
            if (value.Length > 100)
                throw new ArgumentException("O nome da categoria não pode ter mais que 100 caracteres.", nameof(value));

            Value = value.Trim();
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj) =>
            obj is CategoryName other && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();
    }
}