using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.DTOs.Common;

/// <summary>
/// Represents a paginated result set
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public record PagedResult<T>
{
    [JsonPropertyName("items")]
    public IReadOnlyList<T> Items { get; init; } = [];

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalPages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => PageNumber < TotalPages;

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Creates a new paged result
    /// </summary>
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize) => new()
    {
        Items = items,
        TotalCount = totalCount,
        PageNumber = pageNumber,
        PageSize = pageSize
    };

    /// <summary>
    /// Creates an empty paged result
    /// </summary>
    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10) => new()
    {
        Items = [],
        TotalCount = 0,
        PageNumber = pageNumber,
        PageSize = pageSize
    };
}

/// <summary>
/// Pagination request parameters
/// </summary>
public record PageRequest
{
    [JsonPropertyName("pageNumber")]
    public int PageNumber { get; init; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; } = 10;

    [JsonPropertyName("sortBy")]
    public string? SortBy { get; init; }

    [JsonPropertyName("sortDescending")]
    public bool SortDescending { get; init; } = false;

    /// <summary>
    /// Validates the page request and returns normalized values
    /// </summary>
    public PageRequest Normalize() => this with
    {
        PageNumber = Math.Max(1, PageNumber),
        PageSize = Math.Max(1, Math.Min(100, PageSize)) // Limit to max 100 items per page
    };
}