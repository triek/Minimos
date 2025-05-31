using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerMovement : MonoBehaviour
{
    public float moveSpeed = 3;
    public float stopThreshold = 0.01f;

    private enum MovementState { Idling, Moving, Waiting }
    private MovementState currentState = MovementState.Idling;

    private Animator animator;
    private Coroutine currentMovementCoroutine;

    private List<Node> path;
    private int currentPathIndex;

    private static HashSet<Vector3> occupiedPositions = new();
    private static List<Vector3> availablePositions = new List<Vector3>
    {
        new Vector3(-4, 2, 0), new Vector3(-3, 2, 0), new Vector3(-2, 2, 0), new Vector3(-1, 2, 0),new Vector3(2, 2, 0), new Vector3(3, 2, 0), new Vector3(4, 2, 0),
        new Vector3(-4, 1, 0), new Vector3(-3, 1, 0), new Vector3(-2, 1, 0), new Vector3(-1, 1, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(2, 1, 0), new Vector3(3, 1, 0), new Vector3(4, 1, 0), new Vector3(5, 1, 0),
        new Vector3(-4, 0, 0), new Vector3(-3, 0, 0), new Vector3(-2, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(3, 0, 0), new Vector3(4, 0, 0), new Vector3(5, 0, 0),
        new Vector3(-4, -1, 0), new Vector3(-3, -1, 0), new Vector3(-2, -1, 0), new Vector3(-1, -1, 0), new Vector3(0, -1, 0), new Vector3(1, -1, 0), new Vector3(2, -1, 0), new Vector3(3, -1, 0), new Vector3(4, -1, 0), new Vector3(5, -1, 0),
        new Vector3(-3, -2, 0), new Vector3(-2, -2, 0), new Vector3(-1, -2, 0), new Vector3(0, -2, 0), new Vector3(1, -2, 0), new Vector3(2, -2, 0), new Vector3(3, -2, 0), new Vector3(4, -2, 0),
        new Vector3(-2, -3, 0), new Vector3(-1, -3, 0), new Vector3(0, -3, 0), new Vector3(1, -3, 0), new Vector3(2, -3, 0), new Vector3(3, -3, 0),
        new Vector3(0, -4, 0), new Vector3(1, -4, 0)
        //List of available positions for the settler to move to
    };

    private Vector3 currentTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentState == MovementState.Idling)
        {
            StartCoroutine(MoveToAvailablePosition());
            Debug.Log("Settler moving to a defined position");
        }

        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    private void SetMovementState(MovementState state)
    {
        currentState = state;
        animator.SetBool("isMoving", state == MovementState.Moving);
        if (state == MovementState.Idling)
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", -1);
        }
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

    private IEnumerator MoveToAvailablePosition()
    {
        Vector3 targetPos = GetUnoccupiedPosition();
        if (targetPos == Vector3.zero) yield break;

        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        currentMovementCoroutine = StartCoroutine(FindAndMoveToTarget(targetPos));
    }

    private Vector3 GetUnoccupiedPosition()
    {
        foreach (Vector3 pos in availablePositions)
        {
            if (!occupiedPositions.Contains(pos))
            {
                if (currentTarget != Vector3.zero)
                {
                    occupiedPositions.Remove(currentTarget);
                }
                currentTarget = pos;
                occupiedPositions.Add(currentTarget);
                return pos;
            }
        }
        Debug.Log("No available positions left.");
        return Vector3.zero;
    }

    private IEnumerator FindAndMoveToTarget(Vector3 targetPos)
    {
        Node startNode = AStarManager.instance.FindNearestNode(transform.position);
        Node endNode = AStarManager.instance.FindNearestNode(targetPos);

        path = AStarManager.instance.GeneratePath(startNode, endNode);
        currentPathIndex = 0;

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
                }
                currentPathIndex++;
            }
        }

        // Stop moving state
        SetMovementState(MovementState.Waiting);
        yield return new WaitForSeconds(1f);

        // Ensure settler fully stops
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
    }

    private void OnDestroy()
    {
        occupiedPositions.Remove(currentTarget);
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