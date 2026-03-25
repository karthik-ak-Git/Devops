using System.Text.Json.Serialization;

namespace DevOpsAIAgent.Core.DTOs.Common;

/// <summary>
/// Standard API response wrapper for consistent response format
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public record ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<string>? Errors { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResponse<T> SuccessResult(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse<T> ErrorResult(string message, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };

    /// <summary>
    /// Creates an error response with multiple errors
    /// </summary>
    public static ApiResponse<T> ErrorResult(IReadOnlyList<string> errors) => new()
    {
        Success = false,
        Message = "Validation failed",
        Errors = errors
    };
}

/// <summary>
/// Non-generic API response for operations that don't return data
/// </summary>
public record ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<string>? Errors { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse SuccessResult(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    /// <summary>
    /// Creates an error response
    /// </summary>
    public static ApiResponse ErrorResult(string message, IReadOnlyList<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}