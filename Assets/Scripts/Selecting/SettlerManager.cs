using System.Collections.Generic;
using UnityEngine;

public class SettlerManager : MonoBehaviour
{
    public static SettlerManager Instance { get; private set; }

    private readonly List<MovementManager> allSettlers = new List<MovementManager>();
    private readonly List<MovementManager> selectedSettlers = new List<MovementManager>();

    public bool HasSelection => selectedSettlers.Count > 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called by settlers when they spawn.
    /// </summary>
    public void RegisterSettler(MovementManager settler)
    {
        if (!allSettlers.Contains(settler))
        {
            allSettlers.Add(settler);
        }
    }

    /// <summary>
    /// Called when a settler is selected (individually or via Select All).
    /// </summary>
    public void SelectSettler(MovementManager settler)
    {
        if (!selectedSettlers.Contains(settler))
        {
            selectedSettlers.Add(settler);
        }
        UpdateSelectionUI();
    }

    /// <summary>
    /// Called when a settler is deselected.
    /// </summary>
    public void DeselectSettler(MovementManager settler)
    {
        if (selectedSettlers.Contains(settler))
        {
            selectedSettlers.Remove(settler);
        }
        UpdateSelectionUI();
    }

    /// <summary>
    /// Deselect all settlers.
    /// </summary>
    public void DeselectAll()
    {
        foreach (var settler in selectedSettlers)
        {
            settler.SelectCharacter(false);
        }
        selectedSettlers.Clear();
        UpdateSelectionUI();
    }

    /// <summary>
    /// Select all settlers in the scene.
    /// </summary>
    public void SelectAll()
    {
        selectedSettlers.Clear();
        foreach (var settler in allSettlers)
        {
            settler.SelectCharacter(true);
            selectedSettlers.Add(settler);
        }
        UpdateSelectionUI();
    }

    /// <summary>
    /// Call this to update UI whenever selection state changes.
    /// </summary>
    private void UpdateSelectionUI()
    {
        UIManager.Instance?.SetSelectionButtonVisibility(HasSelection);
    }
}
