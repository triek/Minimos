using System.Collections;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private SettlerMovement randomMovement;
    private MouseClickMovement mouseClickMovement;
    private Coroutine countdownCoroutine;
    private Animator animator;

    public bool isSelected = false;

    private void Awake()
    {
        randomMovement = GetComponent<SettlerMovement>();
        mouseClickMovement = GetComponent<MouseClickMovement>();
        animator = GetComponent<Animator>();

        randomMovement.enabled = true;
        mouseClickMovement.enabled = false;

        mouseClickMovement.OnMovementComplete += StartCountdown;
    }

    private void Update()
    {
        var taskExecutor = GetComponent<TaskExecutor>();

        // If a task is running, block all player movement input, dont affect movement components
        if (taskExecutor != null && taskExecutor.IsTaskRunning)
        {
            //Debug.Log("[MovementManager] Task is running, blocking player movement input.");
            return;
        }

        if (isSelected && Input.GetMouseButtonDown(1))
        {
            // Raycast to see what was clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Block movement if a flower was clicked
                if (hit.collider.CompareTag("Flower"))
                {
                    return;
                }
            }

            // Stop any ongoing countdown when player clicks again
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }

            // Switch to MouseClickMovement
            randomMovement.StopMovement();
            randomMovement.enabled = false;
            mouseClickMovement.enabled = true;

            mouseClickMovement.OnMouseClick();
        }
    }

    public void StartCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        countdownCoroutine = StartCoroutine(CountdownWhileDisabled());
    }

    private IEnumerator CountdownWhileDisabled()
    {
        float countdown = 5f;

        while (countdown > 0f)
        {
            //Debug.Log($"Countdown: {countdown:F0}");
            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        // After countdown and no mouse click or selected
        if (!isSelected)
        {
            randomMovement.enabled = true;
        }
    }
    public void SelectCharacter(bool select)
    {
        isSelected = select;
        animator.SetBool("isSelected", isSelected); // Update animator

        if (isSelected)
        {
            randomMovement.enabled = false;  // Stop random movement when selected
        }
        else
        {
            if (!mouseClickMovement.enabled)
            {
                randomMovement.enabled = true;   // Resume random movement if not moving to a destination
            }
        }
    }

}
