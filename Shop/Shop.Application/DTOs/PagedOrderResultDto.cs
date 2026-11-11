using Shop.Application.DTOs.Orders;

namespace Shop.Application.DTOs;

public class PagedOrderResultDto
{
    public IEnumerable<OrderListDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}