using Microsoft.EntityFrameworkCore;
using Portifolio.Dominio.ValueObjects; // Product está aqui
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using Portifolio.Aplicacao.Common;    // Para PagedResult
using Portifolio.Aplicacao.Query;     // Para ProductQuery
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portifolio.Infraestrutura.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Product não tem navegações diretas para incluir no exemplo, mas se tivesse, seriam adicionadas aqui.
            // Ex: .Include(p => p.Category) se Category estivesse diretamente em Product e fosse necessário.
            // No ProductService, o nome da categoria é obtido após carregar o produto, possivelmente através de product.Category.Name.
            // Se a entidade Product tivesse uma propriedade de navegação Category, o Include seria importante.
            // A entidade Product atual tem CategoryId, mas a propriedade Category em si não é carregada automaticamente sem Include.
            // No entanto, a entidade Product.cs que li antes tem `public Category Category { get; private set; }`
            // e `public int CategoryId { get; private set; }`
            // Então, vamos incluir a Categoria.
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Products
                                .Include(p => p.Category)
                                .AsNoTracking();

            // Aplicar filtros (exemplos)
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                queryable = queryable.Where(p => p.Name.Value.Contains(query.Name));
            }
            if (query.CategoryId.HasValue)
            {
                queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
            }
            if (query.IsActive.HasValue)
            {
                queryable = queryable.Where(p => p.IsActive == query.IsActive.Value);
            }

            // Contagem total antes da paginação
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Aplicar ordenação (exemplo)
            // Por enquanto, ordenação fixa. Poderia ser dinâmico baseado nos parâmetros de query.
            queryable = queryable.OrderByDescending(p => p.CreatedAt);

            // Aplicar paginação
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
        }

        public void Delete(Product product)
        {
            // Considerando que Product pode ter um soft delete (IsActive)
            // Se for hard delete: _context.Products.Remove(product);
            // Se for soft delete, a lógica de desativação está na entidade ou serviço.
            // O repositório aqui apenas reflete a ação no DbContext.
            // Se Product.Deactivate() já mudou IsActive, Update é suficiente.
            // Se Delete significa remover do banco:
            _context.Products.Remove(product);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
