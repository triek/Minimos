using System.Collections;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private SettlerMovement randomMovement;
    private MouseClickMovement mouseClickMovement;
    private Coroutine countdownCoroutine;

    private void Awake()
    {
        randomMovement = GetComponent<SettlerMovement>();
        mouseClickMovement = GetComponent<MouseClickMovement>();

        randomMovement.enabled = true;
        mouseClickMovement.enabled = false;

        mouseClickMovement.OnMovementComplete += StartCountdown;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Stop any ongoing countdown (when the player clicks again during countdown)
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

        // When countdown reaches 0 and MouseClickMovement was never enabled, enable randomMovement
        //Debug.Log("Countdown finished - Enabling randomMovement");
        randomMovement.enabled = true;
    }

}
