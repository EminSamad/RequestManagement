using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RequestManagement.Data.Repositories.Interfaces;
using System.Text.Json;

namespace RequestManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;

    public CategoryController(IUnitOfWork unitOfWork, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        const string cacheKey = "categories";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            var cachedCategories = JsonSerializer.Deserialize<object>(cached);
            return Ok(cachedCategories);
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(categories), options);

        return Ok(categories);
    }
}