using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portifolio.Aplicacao.DTOs
{
    public record CategoryDto(
        int Id,
        string Name,
        string Description,
        bool IsActive,
        int? ParentCategoryId,
        string? ParentCategoryName,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        List<CategoryDto> SubCategories
    );
}
