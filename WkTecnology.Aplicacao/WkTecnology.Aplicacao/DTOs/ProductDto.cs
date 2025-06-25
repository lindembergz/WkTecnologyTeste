using System;
using System.Collections.Generic; // Ainda pode ser útil se, no futuro, houver listas de outros DTOs simples

namespace Portifolio.Aplicacao.DTOs
{
    public record ProductDto(
        int Id,
        string Name,
        string? Description, // Description pode ser nula na entidade Product (string.Empty)
        // decimal Price, // Removido
        // string Currency, // Removido
        string Brand,
        string Model,
        int Year,
        string Color,
        // string FuelType, // Removido
        int Mileage,
        bool IsActive,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt,
        DateTime? UpdatedAt
        // List<ProductImageDto> Images, // Removido
        // List<ProductSpecificationDto> Specifications // Removido
    );

    // Se ProductImageDto e ProductSpecificationDto não forem mais usados em nenhum lugar,
    // seus arquivos (se existirem) poderiam ser removidos também.
    // Por enquanto, apenas removendo as referências deles de ProductDto.
}
