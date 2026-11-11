using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.StateCities;
using Shop.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Shop.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StateCitiesController(ShopDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StateCityDto>>> GetAll()
    {
        // واکشی کل لیست به صورت ساده
        var data = await context.StateCities
            .AsNoTracking()
            .Select(x => new StateCityDto(x.Id, x.Title, x.ParentId))
            .ToListAsync();

        return Ok(data);
    }
}
    