// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Collections.Concurrent;

namespace BlazorComponentLibrary.BackgroundServices;

/// <summary>
/// Service for managing background tasks execution.
/// Executes long-running operations asynchronously without blocking requests.
/// Supports task scheduling, progress tracking, and status monitoring.
/// </summary>
public class BackgroundTaskService
{
    private readonly ConcurrentDictionary<string, BackgroundTask> _tasks = new();
    private readonly ILogger<BackgroundTaskService> _logger;
    private readonly CancellationTokenSource _globalCts;

    public BackgroundTaskService(ILogger<BackgroundTaskService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _globalCts = new CancellationTokenSource();
    }

    /// <summary>
    /// Queues a background task for execution.
    /// Returns task ID for status tracking.
    /// </summary>
    public string EnqueueTask(Func<CancellationToken, Task> taskFunc, string taskName = "unnamed")
    {
        if (taskFunc == null)
            throw new ArgumentNullException(nameof(taskFunc));

        var taskId = Guid.NewGuid().ToString();
        var task = new BackgroundTask
        {
            Id = taskId,
            Name = taskName,
            Status = TaskStatus.Queued,
            CreatedAt = DateTime.UtcNow,
            Func = taskFunc
        };

        if (_tasks.TryAdd(taskId, task))
        {
            _logger.LogInformation("Task queued: {TaskName} ({TaskId})", taskName, taskId);
            ExecuteTaskAsync(task);
            return taskId;
        }

        throw new InvalidOperationException("Failed to queue task");
    }

    /// <summary>
    /// Executes a task immediately with timeout.
    /// Useful for fire-and-forget operations.
    /// </summary>
    public async Task ExecuteAsync(Func<CancellationToken, Task> taskFunc, TimeSpan? timeout = null)
    {
        if (taskFunc == null)
            throw new ArgumentNullException(nameof(taskFunc));

        using (var cts = new CancellationTokenSource(timeout ?? TimeSpan.FromHours(1)))
        {
            try
            {
                await taskFunc(cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Task execution timeout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Task execution error");
                throw;
            }
        }
    }

    /// <summary>
    /// Gets status of a queued task by ID.
    /// </summary>
    public BackgroundTask? GetTaskStatus(string taskId)
    {
        if (string.IsNullOrEmpty(taskId))
            return null;

        _tasks.TryGetValue(taskId, out var task);
        return task;
    }

    /// <summary>
    /// Gets all tasks with specified status.
    /// </summary>
    public List<BackgroundTask> GetTasksByStatus(TaskStatus status)
    {
        return _tasks.Values.Where(t => t.Status == status).ToList();
    }

    /// <summary>
    /// Cancels a running task.
    /// </summary>
    public bool CancelTask(string taskId)
    {
        if (string.IsNullOrEmpty(taskId))
            return false;

        if (_tasks.TryGetValue(taskId, out var task))
        {
            task.CancellationTokenSource?.Cancel();
            _logger.LogInformation("Task cancelled: {TaskId}", taskId);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes completed task from tracking.
    /// Frees memory for finished operations.
    /// </summary>
    public bool RemoveTask(string taskId)
    {
        return _tasks.TryRemove(taskId, out _);
    }

    /// <summary>
    /// Gets summary of all active tasks.
    /// </summary>
    public TaskSummary GetSummary()
    {
        var tasks = _tasks.Values.ToList();

        return new TaskSummary
        {
            Total = tasks.Count,
            Queued = tasks.Count(t => t.Status == TaskStatus.Queued),
            Running = tasks.Count(t => t.Status == TaskStatus.Running),
            Completed = tasks.Count(t => t.Status == TaskStatus.Completed),
            Failed = tasks.Count(t => t.Status == TaskStatus.Failed),
            Cancelled = tasks.Count(t => t.Status == TaskStatus.Cancelled)
        };
    }

    /// <summary>
    /// Executes task with status tracking.
    /// Updates status and handles errors.
    /// </summary>
    private void ExecuteTaskAsync(BackgroundTask task)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                task.Status = TaskStatus.Running;
                task.StartedAt = DateTime.UtcNow;
                _logger.LogInformation("Task started: {TaskName} ({TaskId})", task.Name, task.Id);

                using (var cts = new CancellationTokenSource(TimeSpan.FromHours(1)))
                {
                    task.CancellationTokenSource = cts;
                    await task.Func(cts.Token);
                }

                task.Status = TaskStatus.Completed;
                task.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("Task completed: {TaskName} ({TaskId}) in {Duration}ms",
                    task.Name, task.Id, task.Duration?.TotalMilliseconds);
            }
            catch (OperationCanceledException)
            {
                task.Status = TaskStatus.Cancelled;
                task.CompletedAt = DateTime.UtcNow;
                _logger.LogWarning("Task cancelled: {TaskName} ({TaskId})", task.Name, task.Id);
            }
            catch (Exception ex)
            {
                task.Status = TaskStatus.Failed;
                task.CompletedAt = DateTime.UtcNow;
                task.Error = ex.Message;
                _logger.LogError(ex, "Task failed: {TaskName} ({TaskId})", task.Name, task.Id);
            }
        });
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    public void Dispose()
    {
        _globalCts.Cancel();
        _globalCts.Dispose();
        _tasks.Clear();
        _logger.LogInformation("BackgroundTaskService disposed");
    }
}

/// <summary>
/// Represents a background task with metadata.
/// </summary>
public class BackgroundTask
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration => CompletedAt.HasValue && StartedAt.HasValue
        ? CompletedAt.Value - StartedAt.Value
        : null;
    public string? Error { get; set; }
    public Func<CancellationToken, Task>? Func { get; set; }
    public CancellationTokenSource? CancellationTokenSource { get; set; }
}

/// <summary>
/// Task execution status.
/// </summary>
public enum TaskStatus
{
    Queued,
    Running,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// Summary of background task queue state.
/// </summary>
public class TaskSummary
{
    public int Total { get; set; }
    public int Queued { get; set; }
    public int Running { get; set; }
    public int Completed { get; set; }
    public int Failed { get; set; }
    public int Cancelled { get; set; }
}

/// <summary>
/// Extension method to register BackgroundTaskService in dependency injection.
/// </summary>
public static class BackgroundTaskExtensions
{
    public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
    {
        services.AddSingleton<BackgroundTaskService>();
        return services;
    }
}
