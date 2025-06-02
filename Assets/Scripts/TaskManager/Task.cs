using UnityEngine;

public enum TaskType
{
    None,
    ChopTree,
    PickupFlower,
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public class Task
{
    public int TaskNumber { get; set; }

    public string TaskName;
    public TaskType Type;
    public Vector3 TargetPosition;
    public GameObject TargetObject; // Reference to the object to interact with
    public TaskStatus Status;

    public Task(string taskName, TaskType type, Vector3 targetPosition, GameObject targetObject = null)
    {
        TaskName = taskName;
        Type = type;
        TargetPosition = targetPosition;
        TargetObject = targetObject;
        Status = TaskStatus.Pending;
    }

    public void StartTask()
    {
        Status = TaskStatus.InProgress;
        // Optionally, trigger animation or logic here
    }

    public void CompleteTask()
    {
        Status = TaskStatus.Completed;
        // Optionally, trigger completion logic here
    }

    public void FailTask()
    {
        Status = TaskStatus.Failed;
        // Optionally, trigger failure logic here
    }
}
