using UnityEngine;

public class PickupFlower : MonoBehaviour
{
    public Transform headPosition; // Assign in the Inspector or auto-detect
    private Animator animator;

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
                Debug.LogWarning("Head position not set! Flower will attach at default transform.");
                headPosition = transform; // Fallback
            }
        }
    }

    public void Pickup(GameObject flower)
    {
        Debug.Log("Picking up flower: " + flower.name);

        // Attach the flower to the settler's head
        flower.transform.SetParent(headPosition);
        flower.transform.localPosition = new Vector3(0, 0.3f, 0); // Adjust for head placement
        flower.transform.localRotation = Quaternion.identity;
        flower.transform.localScale *= 0.5f; // Resize if needed

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
            Debug.Log("Changed sorting layer of " + flower.name + " to Item");
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on " + flower.name);
        }

        // Trigger the animation when picking up a flower
        if (animator != null)
        {
            animator.SetBool("isPickupFlower", true);
            Debug.Log("isPickupFlower: true");
        }
        else
        {
            Debug.LogWarning("Animator not found on " + gameObject.name);
        }
    }
}
