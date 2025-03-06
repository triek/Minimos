using UnityEngine;

public class HighlightOutline : MonoBehaviour
{
    [SerializeField] private GameObject outline;

    private void Awake()
    {
        if (outline != null)
        {
            outline.SetActive(false);
        }
    }

    public void SetHighlight(bool isActive)
    {
        if (outline != null)
        {
            outline.SetActive(isActive);
        }
    }
}
