using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClickMovement : MonoBehaviour
{
    public float moveSpeed = 3;
    public float stopThreshold = 0.01f;

    private enum MovementState { Idling, Moving, Waiting }
    private MovementState currentState = MovementState.Idling;

    private Animator animator;
    private Coroutine currentMovementCoroutine;

    private Vector3 targetPos;
    private List<Node> path;
    private int currentPathIndex;

    public delegate void MovementComplete();
    public event MovementComplete OnMovementComplete;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnMouseClick();
        }
    }
    public void OnMouseClick()
    {
        SetTargetPos();

        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }

        currentMovementCoroutine = StartCoroutine(FindAndMoveToTarget(targetPos));
    }

    public void SetTargetPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log("Mouse Clicked. Target Position: " + targetPos);
    }

    private void SetMovementState(MovementState state)
    {
        currentState = state;
        animator.SetBool("isMoving", state == MovementState.Moving);
        //Debug.Log("Current state: " + state);
    }

    private IEnumerator FindAndMoveToTarget(Vector3 targetPos)
    {
        // Find the start and end nodes
        Node startNode = AStarManager.instance.FindNearestNode(transform.position);
        Node endNode = AStarManager.instance.FindNearestNode(targetPos);

        // Generate the path
        path = AStarManager.instance.GeneratePath(startNode, endNode);
        currentPathIndex = 0;

        // Move along the path
        if (path != null && path.Count > 0)
        {
            SetMovementState(MovementState.Moving);
            while (currentPathIndex < path.Count)
            {
                Vector3 nextPos = path[currentPathIndex].transform.position;
                Vector3 direction = (nextPos - transform.position).normalized;
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);

                while (Vector3.Distance(transform.position, nextPos) > stopThreshold)
                {
                    transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                    yield return null;

                    // Check if a new target position has been clicked
                    if (Input.GetMouseButtonDown(1))
                    {
                        yield break; // Exit the current coroutine
                    }
                }
                currentPathIndex++;
            }
        }
        SetMovementState(MovementState.Idling);
        currentMovementCoroutine = null;
        this.enabled = false;
        OnMovementComplete?.Invoke(); // Signal that movement is complete
    }
    private void OnDrawGizmos()
    {
        if (path != null && path.Count > 1)
        {
            Gizmos.color = Color.red;
            for (int i = currentPathIndex; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].transform.position, path[i + 1].transform.position);
            }
        }
    }
}
