using System;
using System.Collections.Generic;

namespace Portifolio.Aplicacao.DTOs
{
    public record ProductDto(
        int Id,
        string Name,
        string? Description, 
        string Brand,
        string Model,
        int Year,
        string Color,
        int Mileage,
        bool IsActive,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );

}
