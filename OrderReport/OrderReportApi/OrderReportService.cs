using Microsoft.EntityFrameworkCore;
namespace OrderReportApi
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<OrderListDto>> SearchOrdersAsync(OrderSearchRequest request)
        {
            var query = _context.Orders
                .AsNoTracking()
                .Select(o => new OrderListDto
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer.Name,
                    OrderDate = o.OrderDate,
                    Total = o.Total
                });

            // Search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(o =>
                    o.CustomerName.Contains(term) ||
                    o.OrderId.ToString() == term);
            }

            // Sorting (whitelist columns to avoid injection via reflection)
            query = request.SortBy switch
            {
                "CustomerName" => request.SortDesc ? query.OrderByDescending(o => o.CustomerName) : query.OrderBy(o => o.CustomerName),
                "Total" => request.SortDesc ? query.OrderByDescending(o => o.Total) : query.OrderBy(o => o.Total),
                _ => request.SortDesc ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate)
            };

            // Count BEFORE paging (single round trip, SQL Server does COUNT(*) efficiently)
            var totalCount = await query.CountAsync();

            // Page the query — SQL Server translates this to OFFSET/FETCH
            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<OrderListDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
