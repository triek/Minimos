using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour
{
    [SerializeField] private Button selectAllButton;

    private bool isAllSelected = false;

    private void Start()
    {
        if (selectAllButton == null)
        {
            Debug.LogError("Select All Button is not assigned in the inspector.");
            return;
        }

        selectAllButton.onClick.AddListener(ToggleSelectAll);
        UpdateButtonVisual();
    }

    private void ToggleSelectAll()
    {
        if (SettlerManager.Instance == null)
        {
            Debug.LogError("SettlerManager instance not found in scene.");
            return;
        }

        isAllSelected = !isAllSelected;

        if (isAllSelected)
        {
            Debug.Log("All selected");
            SettlerManager.Instance.SelectAll();
        }
        else
        {
            Debug.Log("All deselected");
            SettlerManager.Instance.DeselectAll();
        }

        UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
        ColorBlock colors = selectAllButton.colors;

        if (isAllSelected)
        {
            colors.normalColor = new Color(0.7f, 0.7f, 1f); // Light blue to indicate selected
        }
        else
        {
            colors.normalColor = Color.white; // Default color
        }

        selectAllButton.colors = colors;
    }
}
