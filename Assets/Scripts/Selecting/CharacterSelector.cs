using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterSelector : MonoBehaviour
{
    private bool areaSelectMode = false;
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;
    private bool ignoreToggleEvent = false;

    [SerializeField] private Image toggleImage;      // The Image component that displays the icon
    [SerializeField] private Sprite iconOff;         // Sprite for button_0
    [SerializeField] private Sprite iconOn;          // Sprite for button_1
    [SerializeField] private Toggle areaSelectToggle;// The Toggle button image

    // Track all selected characters
    private List<MovementManager> selectedCharacters = new List<MovementManager>();

    // Threshold in pixels to distinguish drag from click
    private const float dragThreshold = 10f;

    void Update()
    {
        if (areaSelectMode)
        {
            AreaSelectInput();
        }
        else
        {
            SingleSelectInput();
        }
    }

    private void AreaSelectInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return; // Prevent area select start on UI
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            dragEndPos = Input.mousePosition;
            isDragging = false;

            if (Vector2.Distance(dragStartPos, dragEndPos) > dragThreshold)
            {
                SelectCharactersInRectangle();
                // Stay in area select mode for more drags
            }
            else
            {
                // Treat as a click: turn off area select mode
                SetAreaSelectMode(false);
            }
        }
    }

    private void SingleSelectInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 1. Settler selection
                MovementManager manager = hit.collider.GetComponent<MovementManager>();
                if (manager != null)
                {
                    SelectSingleCharacter(manager);
                    return;
                }

                // 2. Flower selection (do not affect settler selection)
                if (hit.collider.CompareTag("Flower"))
                    return;

                // 3. Background (deselect only if not in flower selection mode)
                if (hit.collider.CompareTag("Background"))
                {
                    if (!FlowerSelector.Instance.IsSelecting)
                    {
                        DeselectAllCharacters();
                    }
                    return;
                }
            }
        }
    }
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void SelectSingleCharacter(MovementManager newSelection)
    {
        DeselectAllCharacters();
        selectedCharacters.Add(newSelection);
        newSelection.SelectCharacter(true);
    }

    void DeselectAllCharacters()
    {
        foreach (var character in selectedCharacters)
        {
            character.SelectCharacter(false);
        }
        selectedCharacters.Clear();
    }

    void SelectCharactersInRectangle()
    {
        Vector2 min = Vector2.Min(dragStartPos, dragEndPos);
        Vector2 max = Vector2.Max(dragStartPos, dragEndPos);
        Rect selectionRect = new Rect(min, max - min);
        
        DeselectAllCharacters();

        foreach (MovementManager manager in FindObjectsByType<MovementManager>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(manager.transform.position);
            if (selectionRect.Contains(screenPos, true))
            {
                manager.SelectCharacter(true);
                selectedCharacters.Add(manager);
            }
            else
            {
                manager.SelectCharacter(false);
            }
        }
    }


    void OnGUI()
    {
        //Debug.Log($"OnGUI called: areaSelectMode={areaSelectMode}, isDragging={isDragging}");
        if (areaSelectMode && isDragging)
        {
            var rect = DrawRect.GetScreenRect(dragStartPos, Input.mousePosition);
            DrawRect.DrawScreenRect(rect, new Color(0.8f, 0.8f, 1f, 0.25f));
            DrawRect.DrawScreenRectBorder(rect, 2, Color.yellow);
        }
    }

    public void EnableAreaSelectMode()
    {
        SetAreaSelectMode(true);
    }

    public void SetAreaSelectMode(bool enabled)
    {
        areaSelectMode = enabled;

        if (toggleImage != null)
        {
            toggleImage.sprite = enabled ? iconOn : iconOff;
        }

        if (areaSelectToggle != null && areaSelectToggle.isOn != enabled)
        {
            ignoreToggleEvent = true;
            areaSelectToggle.isOn = enabled;
            ignoreToggleEvent = false;
        }
    }

    public void OnAreaSelectToggleChanged(bool enabled)
    {
        if (ignoreToggleEvent) return;
        SetAreaSelectMode(enabled);
    }

    public void SelectAllSettlers()
    {
        DeselectAllCharacters();

        foreach (MovementManager manager in FindObjectsByType<MovementManager>(FindObjectsSortMode.None))
        {
            manager.SelectCharacter(true);
            selectedCharacters.Add(manager);
        }
    }



}
