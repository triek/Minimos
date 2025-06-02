using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FlowerSelector : MonoBehaviour
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

    // Track all selected flowers
    private List<GameObject> selectedFlowers = new List<GameObject>();

    // Threshold in pixels to distinguish drag from click
    private const float dragThreshold = 10f;

    public static FlowerSelector Instance { get; private set; }
    public bool IsSelecting => areaSelectMode;
    public IReadOnlyList<GameObject> SelectedFlowers => selectedFlowers;

    private void Awake()
    {
        Instance = this;
    }

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
            if (IsPointerOverUI()) return;
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            if (IsPointerOverUI())
            {
                isDragging = false;
                SetAreaSelectMode(false);
                return;
            }

            dragEndPos = Input.mousePosition;
            isDragging = false;

            if (Vector2.Distance(dragStartPos, dragEndPos) > dragThreshold)
            {
                SelectFlowersInRectangle();
            }
            else
            {
                SetAreaSelectMode(false);
                DeselectAllFlowers();
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
                if (IsFlower(hit.collider.gameObject))
                {
                    SelectSingleFlower(hit.collider.gameObject);
                    return;
                }
            }
            DeselectAllFlowers();
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void SelectSingleFlower(GameObject flower)
    {
        DeselectAllFlowers();
        selectedFlowers.Add(flower);
        SetFlowerSelected(flower, true);
    }

    void DeselectAllFlowers()
    {
        foreach (var flower in selectedFlowers)
        {
            SetFlowerSelected(flower, false);
        }
        selectedFlowers.Clear();
    }

    void SelectFlowersInRectangle()
    {
        Vector2 min = Vector2.Min(dragStartPos, dragEndPos);
        Vector2 max = Vector2.Max(dragStartPos, dragEndPos);
        Rect selectionRect = new Rect(min, max - min);

        DeselectAllFlowers();

        foreach (var flower in FindAllFlowers())
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(flower.transform.position);
            if (selectionRect.Contains(screenPos, true))
            {
                SetFlowerSelected(flower, true);
                selectedFlowers.Add(flower);
                Debug.Log($"Flower selected (area): {flower.name}");
            }
            else
            {
                SetFlowerSelected(flower, false);
            }
        }
    }

    // Helper: Find all flowers in the scene (BoxCollider + SpriteRenderer, not tagged as something else)
    List<GameObject> FindAllFlowers()
    {
        var allColliders = FindObjectsByType<BoxCollider>(FindObjectsSortMode.None);
        var flowers = new List<GameObject>();
        foreach (var col in allColliders)
        {
            var go = col.gameObject;
            if (go.GetComponent<SpriteRenderer>() != null)
            {
                flowers.Add(go);
            }
        }
        return flowers;
    }

    // Helper: Is this GameObject a flower?
    bool IsFlower(GameObject go)
    {
        return go.GetComponent<BoxCollider>() != null && go.GetComponent<SpriteRenderer>() != null;
    }

    // Show/hide the selection icon
    void SetFlowerSelected(GameObject flower, bool selected)
    {
        var icon = flower.transform.Find("SelectedIcon");
        if (icon != null)
        {
            icon.gameObject.SetActive(selected);
        }
    }

    void OnGUI()
    {
        if (areaSelectMode && isDragging)
        {
            var rect = DrawRect.GetScreenRect(dragStartPos, Input.mousePosition);
            DrawRect.DrawScreenRect(rect, new Color(1f, 0.9f, 0.6f, 0.25f));
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

    public void SelectAllFlowers()
    {
        DeselectAllFlowers();

        foreach (var flower in FindAllFlowers())
        {
            SetFlowerSelected(flower, true);
            selectedFlowers.Add(flower);
        }
    }
}
