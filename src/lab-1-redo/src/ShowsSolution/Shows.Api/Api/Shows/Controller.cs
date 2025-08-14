using Marten;
using Microsoft.AspNetCore.Mvc;
using Riok.Mapperly.Abstractions;

namespace Shows.Api.Api.Shows;

public class Controller(IDocumentSession session,TimeProvider clock): ControllerBase
{
    [HttpPost("/api/shows")]
    public async Task<ActionResult> AddAShow(
        [FromBody] ShowCreateRequest request
        )
    {
        var entity = request.MapToEntity(Guid.NewGuid(), clock.GetLocalNow());
        session.Store(entity);
        await session.SaveChangesAsync();
        ShowDetailsResponse response = entity.MapToResponse();
        return Ok(response);
    }

    [HttpGet("/api/shows")]
    public async Task<ActionResult> GetAllShows(CancellationToken token)
    {
        var response = await session.Query<ShowEntity>()
          
            .ProjectToResponse()   // select from the entities a bunch of ShowDetailResponse   
            .ToListAsync(token);

        return Ok(response.OrderByDescending(s => s.CreatedAt));
    }
}


public record ShowCreateRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string StreamingService { get; init; } = string.Empty;
}

public record ShowDetailsResponse
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string StreamingService { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init ; }
}

public record ShowSummaryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class ShowEntity
{
    public Guid Id { get; init; } = Guid.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string StreamingService { get; init; } = string.Empty;

    public DateTimeOffset CreatedAt { get; init; }
}

[Mapper]
public static partial class ShowMappers
{
    public static partial ShowEntity MapToEntity(this ShowCreateRequest request, Guid Id, DateTimeOffset createdAt);
    public static partial ShowDetailsResponse MapToResponse(this ShowEntity entity);

    public static partial IQueryable<ShowDetailsResponse> ProjectToResponse(this IQueryable<ShowEntity> q);
}


