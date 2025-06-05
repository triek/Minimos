using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using TMPro;

public class TaskExecutor : MonoBehaviour
{
    public TMP_Text debugText;
    private PickupFlower pickupFlower;
    private Queue<Task> taskQueue = new Queue<Task>();
    private bool isTaskRunning = false;
    private MovementManager movementManager;
    private Task currentTask; // Track the current task for logging
    private int nextTaskNumber = 1;

    //void Start()
    //{
    //    debugText.text = "Test <color=green>Green</color>!";
    //}

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
        Debug.Log($"[Task #{task.TaskNumber}] <color=blue>Enqueue:</color> {task.TaskName}");
        //debugText.text = "Test <color=green>debugText</color>!";

        taskQueue.Enqueue(task);

        if (!isTaskRunning)
        {
            ExecuteNextTask();
        }
        
        if (debugText == null)
            Debug.LogWarning($"debugText is null in {gameObject.name}'s TaskExecutor");
    }

    private void ExecuteNextTask()
    {
        if (taskQueue.Count > 0)
        {
            isTaskRunning = true;
            currentTask = taskQueue.Dequeue();

            string target = currentTask.TargetObject != null ? currentTask.TargetObject.name : "null";
            Debug.Log($"[Task #{currentTask.TaskNumber}] <color=yellow>Execute:</color> {currentTask.TaskName}");

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
        Debug.Log($"[Task #{currentTask.TaskNumber}] <color=red>Dequeue:</color> {taskName}");
        isTaskRunning = false;
        ExecuteNextTask();
    }
}
