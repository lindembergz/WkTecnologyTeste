namespace Portifolio.Aplicacao.DTOs
{
    public record UpdateCategoryDto(
        string Name,
        string? Description,
        int? ParentCategoryId
        //bool? IsActive 

    );
}
