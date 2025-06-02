using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PickupButtonController : MonoBehaviour
{
    private Action settlerAction;
    private GameObject singleFlowerTarget;
    private List<GameObject> multipleFlowerTargets;
    private bool isMultiple = false;
    private bool _selectionIconHidden = false;

    private void Update()
    {
        // Only check for clicks if the button is active
        if (gameObject.activeInHierarchy && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            // If not clicking on UI (button), destroy this button
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Destroy(gameObject);
            }
        }
    }

    // Show the pickup button for a single flower.
    public void ShowSingle(Action action, GameObject flower, Vector2 screenPosition)
    {
        settlerAction = action;
        singleFlowerTarget = flower;
        isMultiple = false;

        // Show SelectedIcon for the flower
        var icon = flower.transform.Find("SelectedIcon");
        if (icon != null)
            icon.gameObject.SetActive(true);

        SetupButton(screenPosition);
    }

    // Show the pickup button for multiple selected flowers.
    public void ShowMultiple(Action action, List<GameObject> flowers, Vector2 screenPosition)
    {
        settlerAction = action;
        multipleFlowerTargets = flowers;
        isMultiple = true;

        // Show SelectedIcon for all selected flowers
        foreach (var flower in flowers)
        {
            var icon = flower.transform.Find("SelectedIcon");
            if (icon != null)
                icon.gameObject.SetActive(true);
        }

        SetupButton(screenPosition);
    }

    private void SetupButton(Vector2 screenPosition)
    {
        // Position the button on the canvas
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition, canvas.worldCamera, out pos);
        // Offset the button down by 5 pixels
        pos.y -= 5f;
        rect.anchoredPosition = pos;

        // Add click event
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnPickupButtonClicked);
    }

    private void OnPickupButtonClicked()
    {
        if (isMultiple)
        {
            OnPickupMultipleFlowersClicked();
        }
        else
        {
            OnPickupSingleFlowerClicked();
        }
        Destroy(gameObject); // Remove the button after click
    }

    private void OnPickupSingleFlowerClicked()
    {
        if (settlerAction != null && singleFlowerTarget != null)
        {
            settlerAction.QueuePickupTasks(new List<GameObject> { singleFlowerTarget });

            // Hide SelectedIcon
            var icon = singleFlowerTarget.transform.Find("SelectedIcon");
            if (icon != null)
                icon.gameObject.SetActive(false);

            _selectionIconHidden = true;
        }
    }

    private void OnPickupMultipleFlowersClicked()
    {
        if (settlerAction != null && multipleFlowerTargets != null && multipleFlowerTargets.Count > 0)
        {
            settlerAction.QueuePickupTasks(multipleFlowerTargets);

            // Hide SelectedIcon for all
            foreach (var flower in multipleFlowerTargets)
            {
                var icon = flower.transform.Find("SelectedIcon");
                if (icon != null)
                    icon.gameObject.SetActive(false);
            }

            _selectionIconHidden = true;
        }
    }

    private void OnDestroy()
    {
        if (!_selectionIconHidden)
        {
            if (!isMultiple && singleFlowerTarget != null)
            {
                var icon = singleFlowerTarget.transform.Find("SelectedIcon");
                if (icon != null)
                    icon.gameObject.SetActive(false);
            }
            else if (isMultiple && multipleFlowerTargets != null)
            {
                foreach (var flower in multipleFlowerTargets)
                {
                    var icon = flower.transform.Find("SelectedIcon");
                    if (icon != null)
                        icon.gameObject.SetActive(false);
                }
            }
        }
    }

}
