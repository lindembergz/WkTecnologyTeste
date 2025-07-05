namespace Portifolio.Aplicacao.DTOs
{
    public record CreateCategoryDto(
        string Name,
        string? Description,
        int? ParentCategoryId
    );
}
