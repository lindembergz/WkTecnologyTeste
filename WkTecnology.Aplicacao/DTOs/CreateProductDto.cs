namespace Portifolio.Aplicacao.DTOs
{
    public record CreateProductDto(
        string Name,
        string? Description,
        string Brand,
        string Model,
        int Year,
        string Color,
        int Mileage,
        int CategoryId,
        bool IsActive = true 
    );
}
