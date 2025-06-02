using UnityEngine;
using System.Collections;

public class PickupFlower : MonoBehaviour
{
    private Animator animator;

    public Transform headPosition; // Assign in the Inspector or auto-detect
    public System.Action OnTaskComplete;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (headPosition == null)
        {
            // Try to find a child object named "Head"
            Transform head = transform.Find("Head");
            if (head != null)
            {
                headPosition = head;
            }
            else
            {
                headPosition = transform; // Fallback
            }
        }
    }

    // Called by TaskExecutor to execute a pickup flower task.
    public void Execute(MovementManager movementManager, Task task)
    {
        var randomMovement = movementManager.GetComponent<SettlerMovement>();
        var mouseClickMovement = movementManager.GetComponent<MouseClickMovement>();

        // Disable random movement, enable mouse click movement
        randomMovement.StopMovement();
        randomMovement.enabled = false;
        mouseClickMovement.enabled = true;

        // Move to the task position using mouse click movement
        mouseClickMovement.SetTargetPosForTask(task.TargetPosition);

        // Start the pickup sequence
        StartCoroutine(PickupSequence(movementManager, task));
    }

    private IEnumerator PickupSequence(MovementManager movementManager, Task task)
    {
        var mouseClickMovement = movementManager.GetComponent<MouseClickMovement>();
        if (mouseClickMovement == null)
        {
            Debug.LogError("MouseClickMovement not found on settler.");
            task.FailTask();
            yield break;
        }

        // 1. Move to the flower's position
        Vector3 targetPos = task.TargetObject.transform.position;
        bool arrived = false;

        void OnArrived()
        {
            arrived = true;
            mouseClickMovement.OnMovementComplete -= OnArrived;
        }

        mouseClickMovement.OnMovementComplete += OnArrived;

        // Wait until arrived
        while (!arrived)
            yield return null;

        // 2. Wait 1 seconds
        yield return new WaitForSeconds(1f);

        // 3. Pickup the flower
        Pickup(task.TargetObject);
        task.CompleteTask();

        // 4. Notify TaskExecutor that the task is complete
        OnTaskComplete?.Invoke();
    }

    public void Pickup(GameObject flower)
    {
        // Attach the flower to the settler's head
        flower.transform.SetParent(headPosition);
        flower.transform.localPosition = new Vector3(0, 0.3f, 0); // Adjust for head placement
        flower.transform.localRotation = Quaternion.identity;
        flower.transform.localScale *= 1f; // Resize if needed

        // Disable physics to prevent falling
        if (flower.TryGetComponent(out Collider flowerCollider))
        {
            flowerCollider.enabled = false;
        }
        if (flower.TryGetComponent(out Rigidbody flowerRigidbody))
        {
            Destroy(flowerRigidbody);
        }

        // Change sorting layer to "Item"
        SpriteRenderer flowerRenderer = flower.GetComponent<SpriteRenderer>();
        if (flowerRenderer != null)
        {
            flowerRenderer.sortingLayerName = "Item";
        }

        // Trigger the animation when picking up a flower
        if (animator != null)
        {
            animator.SetBool("isPickupFlower", true);
        }
    }
}
