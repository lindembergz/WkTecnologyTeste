using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portifolio.Core;

namespace Portifolio.Domain.Query
{
    public class ProductQuery : PagedQuery
    {
        public int? CategoryId { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }

        public ProductQuery() { }

        public ProductQuery(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null, bool sortDescending = false)
            : base(page, pageSize, search, sortBy, sortDescending) 
        {
            CategoryId = null;
            Name = null;
            IsActive = null;
        }
    }
}


