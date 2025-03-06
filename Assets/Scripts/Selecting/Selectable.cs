using UnityEngine;

public class Selectable : MonoBehaviour
{
    private bool isSelected = false;
    private HighlightOutline highlightOutline;

    private void Awake()
    {
        highlightOutline = GetComponent<HighlightOutline>();
        if (highlightOutline != null)
        {
            highlightOutline.SetHighlight(false);
        }
    }

    private void OnMouseDown()
    {
        ToggleSelection();
    }

    private void ToggleSelection()
    {
        isSelected = !isSelected;
        if (highlightOutline != null)
        {
            highlightOutline.SetHighlight(isSelected);
        }
    }
}
