using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WkTecnology.Core
{
    public class PagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
        public int PageCount { get; set; }
        public int TotalCount { get; set; }
        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
        public int Count => (Page - 1) * PageSize + 1;
        public List<string> Query { get; set; } = new List<string>();
        public List<string> Sort { get; set; } = new List<string>();

        public PagedQuery() { }
        public PagedQuery(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null, bool sortDescending = false)
        {
            Page = page;
            PageSize = pageSize;
            Search = search;
            SortBy = sortBy;
            SortDescending = sortDescending;
            if (!string.IsNullOrWhiteSpace(search))
            {
                Query.Add($"search={search}");
            }
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                Sort.Add($"{sortBy}{(sortDescending ? " desc" : "")}");
            }
        }
    }
}





