using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.EventSystems;

public class Action : MonoBehaviour
{
    private MovementManager movementManager;
    private MouseClickMovement mouseClickMovement;
    private ResourceManager resourceManager;
    private ResourceManager.ResourceType resourceType;
    private GameObject pendingTargetObject;
    private PickupFlower pickupFlower;

    public GameObject pickupButtonPrefab; // Assign in Inspector

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        mouseClickMovement = GetComponent<MouseClickMovement>();
        pickupFlower = GetComponent<PickupFlower>();
    }

    private void Update()
    {
        if (!IsSelected())
            return;

        // Handle right-click (show pickup button for single flower)
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Flower"))
                {
                    // Show the pickup button for a single flower using PickupButtonController
                    var canvas = FindFirstObjectByType<Canvas>();
                    var buttonObj = Instantiate(
                        pickupButtonPrefab, // Use the prefab assigned in the Inspector on this Action component
                        canvas.transform
                    );
                    var controller = buttonObj.GetComponent<PickupButtonController>();
                    controller.ShowSingle(this, hit.collider.gameObject, Input.mousePosition);
                    return;
                }
                
                // Only allow movement to ground/other objects if not busy
                var taskExecutor = GetComponent<TaskExecutor>();
                if (taskExecutor != null && taskExecutor.IsTaskRunning)
                {
                    return;
                }

                if (hit.collider.CompareTag("Tree"))
                {
                    SetHarvestTarget(hit.collider.gameObject, ResourceManager.ResourceType.Wood);
                }
                else if (hit.collider.CompareTag("Stone"))
                {
                    SetHarvestTarget(hit.collider.gameObject, ResourceManager.ResourceType.Stone);
                }

                else
                {
                    // Debug.Log("[Action] Right-clicked non-flower object.");
                }
            }
        }
    }

    private bool IsSelected()
    {
        return movementManager != null && movementManager.isSelected;
    }

    private void SetHarvestTarget(GameObject target, ResourceManager.ResourceType type)
    {
        resourceType = type;
        pendingTargetObject = target;
        mouseClickMovement.OnMovementComplete += HandleMovementComplete;
    }

    private void HandleMovementComplete()
    {
        mouseClickMovement.OnMovementComplete -= HandleMovementComplete;
        if (pendingTargetObject != null)
        {
            StartCoroutine(HarvestCoroutine(pendingTargetObject));
            pendingTargetObject = null; // Clear after use
        }
        else
        {
            Debug.Log("No target object set when trying to harvest!");
        }
    }

    private IEnumerator HarvestCoroutine(GameObject harvestedObject)
    {
        Debug.Log("Harvesting: " + harvestedObject.name);
        yield return new WaitForSeconds(1f);

        // If it's a flower, call PickupFlower script
        if (harvestedObject.CompareTag("Flower") && pickupFlower != null)
        {
            pickupFlower.Pickup(harvestedObject);
        }
        else
        {
            resourceManager = FindResourceManager(resourceType);
            if (resourceManager != null)
            {
                resourceManager.AddResource(10);
                Destroy(harvestedObject);
            }
            else
            {
                Debug.LogError("No ResourceManager available to add resources!");
            }
        }
    }

    private ResourceManager FindResourceManager(ResourceManager.ResourceType type)
    {
        ResourceManager[] allManagers = FindObjectsByType<ResourceManager>(FindObjectsSortMode.None);

        foreach (var manager in allManagers)
        {
            if (manager.resourceType == type)
            {
                return manager;
            }
        }
        Debug.LogError($"ResourceManager for {type} not found!");
        return null;
    }

    public void QueuePickupTasks(IEnumerable<GameObject> flowers)
    {
        var taskExecutor = GetComponent<TaskExecutor>();
        foreach (var flower in flowers)
        {
            var pickupTask = new Task(
                "Pickup Flower",
                TaskType.PickupFlower,
                flower.transform.position,
                flower
            );
            taskExecutor.EnqueueTask(pickupTask);
        }
    }
}