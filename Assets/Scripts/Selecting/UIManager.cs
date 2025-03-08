using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject selectionIndicatorButton;

    private void Awake()
    {
        Instance = this;
        selectionIndicatorButton?.SetActive(false);
    }

    public void SetSelectionButtonVisibility(bool isVisible)
    {
        if (selectionIndicatorButton != null)
        {
            selectionIndicatorButton.SetActive(isVisible);
        }
    }
}
