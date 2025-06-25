using System.Collections.Generic; // Pode não ser mais necessário aqui

namespace Portifolio.Aplicacao.DTOs
{
    public record CreateProductDto(
        string Name,
        string? Description,
        // decimal Price, // Removido
        string Brand,
        string Model,
        int Year,
        string Color,
        // int FuelType, // Removido (tipo numérico, diferente de string no ProductDto original)
        int Mileage,
        int CategoryId
        // List<string>? ImageUrls = null, // Removido
        // List<ProductSpecificationDto>? Specifications = null // Garantindo que não está aqui
    );
}
