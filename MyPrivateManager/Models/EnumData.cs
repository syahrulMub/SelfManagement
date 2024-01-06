namespace MyPrivateManager.Models;

public enum TaskPriority
{
    Critical = 0,
    High = 1,
    Medium = 2,
    Low = 3
}

public enum TaskStage
{
    NotStarted = 0,
    InProgress = 1,
    Reviewing = 2,
    Completed = 3
}
