using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerMovement : MonoBehaviour
{
    public float moveSpeed = 3;
    public int moveRangeX = 18;
    public int moveRangeY = 10;
    public float stopThreshold = 0.01f;

    private enum MovementState { Idling, Moving, Waiting }
    private MovementState currentState = MovementState.Idling;

    private Animator animator;
    private Coroutine currentMovementCoroutine;

    private List<Node> path;
    private int currentPathIndex;

    private static HashSet<Vector3> occupiedPositions = new();

    private bool isTaskMovement = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentState == MovementState.Idling && !isTaskMovement)
        {
            StartCoroutine(getPos());
            //Debug.Log("Settler wandering");
        }
    }

    private void SetMovementState(MovementState state)
    {
        currentState = state;
        animator.SetBool("isMoving", state == MovementState.Moving);
        //Debug.Log("Current state: " + state);

        // Temporary fix for animation
        //-----------------------------------
        if (state == MovementState.Idling)
        {
            // Face downward when idle
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", -1);
        }
        //-----------------------------------
    }
    public void StopMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }
        SetMovementState(MovementState.Idling);
    }

    IEnumerator getPos()
    {

        float randomX = Random.Range(-moveRangeX, moveRangeX);
        float randomY = Random.Range(-moveRangeY, moveRangeY);

        Vector3 targetPos = new Vector3(
            randomX,
            randomY,
            transform.position.z
        );
        //Debug.Log("Target position: " + targetPos);

        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }

        currentMovementCoroutine = StartCoroutine(FindAndMoveToTarget(targetPos));
        yield return null;
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
            Vector3 lastValidPos = transform.position;

            while (currentPathIndex < path.Count)
            {
                Vector3 nextPos = path[currentPathIndex].transform.position;

                if (currentPathIndex == path.Count - 1 && occupiedPositions.Contains(nextPos))
                {
                    Debug.Log("Final destination occupied, finding a new one.");
                    yield return StartCoroutine(getPos()); // Find a new random position instead
                    yield break;
                }

                Vector3 direction = (nextPos - transform.position).normalized;
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);

                while (Vector3.Distance(transform.position, nextPos) > stopThreshold)
                {
                    transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                currentPathIndex++;
            }
        }

        SetMovementState(MovementState.Waiting);
        yield return new WaitForSeconds(1f);

        SetMovementState(MovementState.Idling);
        isTaskMovement = false;
    }

    public void MoveTo(Vector3 targetPos)
    {
        isTaskMovement = true;

        if (currentMovementCoroutine != null)
            StopCoroutine(currentMovementCoroutine);

        currentMovementCoroutine = StartCoroutine(FindAndMoveToTarget(targetPos));
    }


    private void OnDestroy()
    {
        occupiedPositions.Remove(transform.position); // Free up space when settler is destroyed
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
