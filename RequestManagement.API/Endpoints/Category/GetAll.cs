using FastEndpoints;
using Microsoft.Extensions.Caching.Distributed;
using RequestManagement.Domain.Interfaces;
using System.Text.Json;

namespace RequestManagement.API.Endpoints.Category;

public class GetAllCategoriesEndpoint : EndpointWithoutRequest
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _cache;

    public GetAllCategoriesEndpoint(IUnitOfWork unitOfWork, IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public override void Configure()
    {
        Get("/api/category");
        Roles("Admin", "Requester", "Executor");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string cacheKey = "categories";

        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (cached != null)
        {
            var cachedCategories = JsonSerializer.Deserialize<object>(cached);
            await HttpContext.Response.WriteAsJsonAsync(cachedCategories!, ct);
            return;
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(categories), options, ct);
        await HttpContext.Response.WriteAsJsonAsync(categories, ct);
    }
}