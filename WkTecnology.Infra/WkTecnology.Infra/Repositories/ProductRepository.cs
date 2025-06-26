using Microsoft.EntityFrameworkCore;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using Portifolio.Core;    // Para PagedResult
                          // Para ProductQuery
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Portifolio.Domain.Query;
using Portifolio.Dominio.Entidades;


namespace Portifolio.Infraestrutura.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbContext _context;

        public ProductRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Product>()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Set<Product>().AddAsync(product, cancellationToken);
        }

        public void Update(Product product)
        {
            _context.Set<Product>().Update(product);
        }

        public void Delete(Product product)
        {
            _context.Set<Product>().Remove(product);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken cancellationToken = default)
        {
            var queryable = _context.Set<Product>()
                .Include(p => p.Category)
                .AsNoTracking();

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

            var totalCount = await queryable.CountAsync(cancellationToken);

            queryable = queryable.OrderByDescending(p => p.Id);

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
    }
}
