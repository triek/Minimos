using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TaskExecutor : MonoBehaviour
{
    private PickupFlower pickupFlower;
    private Queue<Task> taskQueue = new Queue<Task>();
    private bool isTaskRunning = false;
    private MovementManager movementManager;
    private Task currentTask; // Track the current task for logging
    private int nextTaskNumber = 1;

    public bool IsTaskRunning => isTaskRunning;

    private void Awake()
    {
        pickupFlower = GetComponent<PickupFlower>();
        movementManager = GetComponent<MovementManager>();
    }

    public void EnqueueTask(Task task)
    {
        task.TaskNumber = nextTaskNumber++;
        string target = task.TargetObject != null ? task.TargetObject.name : "null";
        Debug.Log($"[Task #{task.TaskNumber}] Enqueue: {task.TaskName} for {target}");
        taskQueue.Enqueue(task);

        if (!isTaskRunning)
        {
            ExecuteNextTask();
        }
    }

    private void ExecuteNextTask()
    {
        if (taskQueue.Count > 0)
        {
            isTaskRunning = true;
            currentTask = taskQueue.Dequeue();

            string target = currentTask.TargetObject != null ? currentTask.TargetObject.name : "null";
            Debug.Log($"[Task #{currentTask.TaskNumber}] Execute: {currentTask.TaskName} for {target}");

            switch (currentTask.Type)
            {
                case TaskType.PickupFlower:
                    pickupFlower.OnTaskComplete = OnTaskComplete; // Set callback
                    pickupFlower.Execute(movementManager, currentTask);
                    break;
            }
        }
        else
        {
            isTaskRunning = false;
        }
    }

    // This should be called by PickupFlower when the task is done
    private void OnTaskComplete()
    {
        string target = currentTask != null && currentTask.TargetObject != null ? currentTask.TargetObject.name : "null";
        string taskName = currentTask != null ? currentTask.TaskName : "null";
        Debug.Log($"[Task #{currentTask.TaskNumber}] Finished: {taskName} for {target}");
        isTaskRunning = false;
        ExecuteNextTask();
    }
}
