// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Generic result wrapper for standardized API responses.
/// Contains success/failure status, data, and error information.
/// </summary>
public class Result<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("data")]
    public T? Data { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("errors")]
    public Dictionary<string, string>? Errors { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static Result<T> SuccessResult(T data, string? message = null)
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static Result<T> FailureResult(string message, Dictionary<string, string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Non-generic result for operations without return data.
/// </summary>
public class Result
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("errors")]
    public Dictionary<string, string>? Errors { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static Result SuccessResult(string? message = null)
    {
        return new Result
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static Result FailureResult(string message, Dictionary<string, string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Result for batch operations returning multiple items.
/// </summary>
public class BatchResult<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("items")]
    public List<T> Items { get; set; } = new();

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("succeeded")]
    public int SucceededCount { get; set; }

    [JsonProperty("failed")]
    public int FailedCount { get; set; }

    [JsonProperty("errors")]
    public List<BatchError>? Errors { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static BatchResult<T> SuccessResult(List<T> items)
    {
        return new BatchResult<T>
        {
            Success = true,
            Items = items,
            Total = items.Count,
            SucceededCount = items.Count,
            FailedCount = 0,
            Timestamp = DateTime.UtcNow
        };
    }

    public static BatchResult<T> PartialResult(List<T> items, List<BatchError> errors)
    {
        return new BatchResult<T>
        {
            Success = false,
            Items = items,
            Total = items.Count + errors.Count,
            SucceededCount = items.Count,
            FailedCount = errors.Count,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class BatchError
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    [JsonProperty("code")]
    public string? Code { get; set; }
}
