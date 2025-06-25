using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portifolio.Aplicacao.DTOs
{
    public record ProductDto(
        int Id,
        string Name,
        string Description,
        decimal Price,
        string Currency,
        string Brand,
        string Model,
        int Year,
        string Color,
        string FuelType,
        int Mileage,
        bool IsActive,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
