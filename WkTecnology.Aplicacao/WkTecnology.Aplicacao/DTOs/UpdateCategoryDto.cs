namespace Portifolio.Aplicacao.DTOs
{
    public record UpdateCategoryDto(
        string Name,
        string? Description,
        int? ParentCategoryId
        // bool? IsActive // Opcional, se a ativação/desativação for feita por aqui também
    );
}
