﻿using System.Collections.Generic;
using WkTecnology.Core;

namespace Portifolio.Dominio.ValueObjects
{
    public class CategoryName : ValueObject
    {
        public string Value { get; }

        private CategoryName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Nome da categoria não pode ser vazia.", nameof(value));
            }

            Value = value;
        }

        public static CategoryName Create(string value)
        {
            return new CategoryName(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }


    }
}
