namespace OrderReportApi
{
    public class OrderListDto
    {
        public int OrderId { get; set; }
        public required string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    // Dtos/OrderSearchRequest.cs
    public class OrderSearchRequest
    {
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public string? SortBy { get; set; } = "OrderDate";
        public bool SortDesc { get; set; } = true;
    }
}
