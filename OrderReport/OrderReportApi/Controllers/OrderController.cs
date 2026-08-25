using Microsoft.AspNetCore.Mvc;

namespace OrderReportApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderListDto>>> Search([FromQuery] OrderSearchRequest request)
        {
            var result = await _orderService.SearchOrdersAsync(request);
            return Ok(result);
        }
    }
}
