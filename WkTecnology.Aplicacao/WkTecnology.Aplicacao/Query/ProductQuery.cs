using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portifolio.Aplicacao.Common;

namespace Portifolio.Aplicacao.Query
{
    public class ProductQuery : PagedQuery
    {
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public List<int>? FuelTypes { get; set; }
    }
}
