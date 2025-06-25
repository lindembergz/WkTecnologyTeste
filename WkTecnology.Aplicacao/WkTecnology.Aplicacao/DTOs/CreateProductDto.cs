using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portifolio.Aplicacao.DTOs
{
    public record CreateProductDto(
        string Name,
        string Description,
        decimal Price,
        string Brand,
        string Model,
        int Year,
        string Color,
        int FuelType,
        int Mileage,
        int CategoryId,
        List<string>? ImageUrls = null

    );
}
