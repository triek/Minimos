using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerMovement : MonoBehaviour
{
    public float moveSpeed = 3;
    public int moveRangeX = 10;
    public int moveRangeY = 4;
    public float stopThreshold = 0.01f;

    private enum MovementState { Idling, Moving, Waiting }
    private MovementState currentState = MovementState.Idling;

    private Animator animator;
    private Coroutine currentMovementCoroutine;

    private Vector3 targetPos;
    private List<Node> path;
    private int currentPathIndex;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentState == MovementState.Idling)
        {
            StartCoroutine(getPos());
            Debug.Log("Settler wandering");
        }
    }
    private void SetMovementState(MovementState state)
    {
        currentState = state;
        animator.SetBool("isMoving", state == MovementState.Moving);
        //Debug.Log("Current state: " + state);
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

        animator.SetFloat("moveX", randomX);
        animator.SetFloat("moveY", randomY);

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

        SetMovementState(MovementState.Waiting);
        yield return new WaitForSeconds(1f);

        SetMovementState(MovementState.Idling);

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
