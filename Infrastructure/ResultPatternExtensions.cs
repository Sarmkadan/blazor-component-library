// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Infrastructure;

/// <summary>
/// Extension methods for Result pattern operations.
/// Enables fluent handling of success/failure cases.
/// Inspired by railway-oriented programming.
/// </summary>
public static class ResultPatternExtensions
{
    /// <summary>
    /// Maps success result to another result type.
    /// Chains operations transforming data at each step.
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TNew>> mapper)
    {
        var result = await resultTask;
        if (result.IsSuccess && result.Data != null)
        {
            var newData = await mapper(result.Data);
            return Result<TNew>.Success(newData);
        }

        return Result<TNew>.Failure(result.Error ?? "Mapping failed");
    }

    /// <summary>
    /// Binds (flatMaps) two Result operations.
    /// Useful for chaining operations that return Results.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNew>>> binder)
    {
        var result = await resultTask;
        if (result.IsSuccess && result.Data != null)
        {
            return await binder(result.Data);
        }

        return Result<TNew>.Failure(result.Error ?? "Binding failed");
    }

    /// <summary>
    /// Executes side effect for successful result.
    /// Useful for logging, saving state, etc.
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task> effect) where T : class
    {
        var result = await resultTask;
        if (result.IsSuccess && result.Data != null)
        {
            await effect(result.Data);
        }

        return result;
    }

    /// <summary>
    /// Executes side effect for failed result.
    /// Useful for error handling and cleanup.
    /// </summary>
    public static async Task<Result<T>> TapErrorAsync<T>(
        this Task<Result<T>> resultTask,
        Func<string, Task> effect) where T : class
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            await effect(result.Error ?? "Unknown error");
        }

        return result;
    }

    /// <summary>
    /// Recovers from failure by providing fallback value.
    /// Continues pipeline with fallback data.
    /// </summary>
    public static async Task<Result<T>> RecoverAsync<T>(
        this Task<Result<T>> resultTask,
        Func<string, Task<T>> fallback) where T : class
    {
        var result = await resultTask;
        if (!result.IsSuccess)
        {
            var recoveredData = await fallback(result.Error ?? "Unknown error");
            return Result<T>.Success(recoveredData);
        }

        return result;
    }

    /// <summary>
    /// Ensures condition is met, returns failure otherwise.
    /// Useful for validation checks.
    /// </summary>
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> condition,
        string errorMessage) where T : class
    {
        if (!result.IsSuccess || result.Data == null)
            return result;

        if (!condition(result.Data))
            return Result<T>.Failure(errorMessage);

        return result;
    }

    /// <summary>
    /// Transforms failure to another result or recovery.
    /// Handles errors gracefully with custom logic.
    /// </summary>
    public static Result<T> OnFailure<T>(
        this Result<T> result,
        Func<string, T> recovery) where T : class
    {
        if (result.IsSuccess)
            return result;

        var recoveredData = recovery(result.Error ?? "Unknown error");
        return Result<T>.Success(recoveredData);
    }

    /// <summary>
    /// Converts Result<T> to nullable T.
    /// Returns data if success, null if failure.
    /// </summary>
    public static T? ToNullable<T>(this Result<T> result) where T : class
    {
        return result.IsSuccess ? result.Data : null;
    }

    /// <summary>
    /// Converts Result to boolean.
    /// Useful for simple success/failure checks.
    /// </summary>
    public static bool IsSuccessful<T>(this Result<T> result) where T : class
    {
        return result.IsSuccess;
    }

    /// <summary>
    /// Throws exception if result is failure.
    /// Stops pipeline with detailed error message.
    /// </summary>
    public static Result<T> ThrowIfFailure<T>(this Result<T> result) where T : class
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException(result.Error ?? "Operation failed");

        return result;
    }

    /// <summary>
    /// Combines multiple results, returns failure if any fails.
    /// Useful for validating multiple operations.
    /// </summary>
    public static Result<List<T>> CombineResults<T>(
        this IEnumerable<Result<T>> results) where T : class
    {
        var resultList = results.ToList();
        var failedResult = resultList.FirstOrDefault(r => !r.IsSuccess);

        if (failedResult != null)
            return Result<List<T>>.Failure(failedResult.Error ?? "One or more operations failed");

        var data = resultList
            .Where(r => r.Data != null)
            .Select(r => r.Data!)
            .ToList();

        return Result<List<T>>.Success(data);
    }

    /// <summary>
    /// Filters results based on predicate.
    /// </summary>
    public static Result<T> Filter<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        string errorMessage) where T : class
    {
        if (!result.IsSuccess || result.Data == null)
            return result;

        return predicate(result.Data)
            ? result
            : Result<T>.Failure(errorMessage);
    }

    /// <summary>
    /// Retries operation on failure up to specified attempts.
    /// Useful for transient failures.
    /// </summary>
    public static async Task<Result<T>> RetryAsync<T>(
        this Func<Task<Result<T>>> operation,
        int maxAttempts = 3,
        int delayMs = 100) where T : class
    {
        Result<T>? lastResult = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            lastResult = await operation();
            if (lastResult.IsSuccess)
                return lastResult;

            if (attempt < maxAttempts)
                await Task.Delay(delayMs * attempt); // Exponential backoff
        }

        return lastResult ?? Result<T>.Failure("Operation exhausted all retries");
    }
}

/// <summary>
/// Standard Result pattern for success/failure handling.
/// Provides functional programming style error handling.
/// </summary>
public class Result<T> where T : class
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }

    private Result(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, null, error);

    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {Data?.GetType().Name}"
            : $"Failure: {Error}";
    }
}

/// <summary>
/// Non-generic Result for operations without return value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);

    public override string ToString()
    {
        return IsSuccess ? "Success" : $"Failure: {Error}";
    }
}
