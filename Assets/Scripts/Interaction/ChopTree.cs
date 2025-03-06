using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTree : MonoBehaviour
{
    private MovementManager movementManager;
    private MouseClickMovement mouseClickMovement;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        mouseClickMovement = GetComponent<MouseClickMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hitted: " + hit.collider.name);
                if (hit.collider.CompareTag("Tree"))
                {
                    mouseClickMovement.OnMovementComplete += HandleMovementComplete;
                }
            } else
            {
                Debug.Log("Raycast missed");
            }
        }
    }

    private void HandleMovementComplete()
    {
        StartCoroutine(ChopTreeCoroutine());
    }

    private IEnumerator ChopTreeCoroutine()
    {
        // Display "chopping tree" message
        Debug.Log("chopping tree");
        mouseClickMovement.OnMovementComplete -= HandleMovementComplete;

        // Wait for an additional 1 second before allowing further interactions
        yield return new WaitForSeconds(1f);
    }
}
// Q: After message chopping tree, the tree disappears, increase the player's wood resource by 1, how can I do it
// A: You can create a new script called Resource.cs in the Storage folder. This script will store the player's resources.
// Q: How can I increase the player's wood resource by 1
