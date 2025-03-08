using System.Collections;
using System.Resources;
using UnityEngine;

public class ChopTree : MonoBehaviour
{
    private MovementManager movementManager;
    private MouseClickMovement mouseClickMovement;
    private ResourceManager resourceManager;
    private ResourceManager.ResourceType resourceType;
    private GameObject pendingTargetObject;
    private PickupFlower pickupFlower;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        mouseClickMovement = GetComponent<MouseClickMovement>();
        pickupFlower = GetComponent<PickupFlower>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Debug.Log("Raycast hitted: " + hit.collider.name);
                if (hit.collider.CompareTag("Tree"))
                {
                    SetHarvestTarget(hit.collider.gameObject, ResourceManager.ResourceType.Wood);
                }
                else if (hit.collider.CompareTag("Stone"))
                {
                    SetHarvestTarget(hit.collider.gameObject, ResourceManager.ResourceType.Stone);

                }
            }
            else
            {
                Debug.Log("Raycast missed");
            }
        }
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

        // If it's a flower (tree), call PickupFlower script
        if (harvestedObject.CompareTag("Tree") && pickupFlower != null)
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
}