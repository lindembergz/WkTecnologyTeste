using Microsoft.EntityFrameworkCore;
using Portifolio.Dominio.Entidades;
using Portifolio.Dominio.Repositories;
using Portifolio.Infraestrutura.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portifolio.Infraestrutura.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PortifolioDbContext _context;

        public CategoryRepository(PortifolioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetSubcategoriesAsync(int parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategories)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Delete(Category category)
        {
            _context.Categories.Remove(category);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
