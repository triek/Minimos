using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resource : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceCount = 0;
    public TMP_Text resourceCountText;

    private void Start()
    {
        UpdateResourceCountUI();
    }

    public void AddResource(int amount)
    {
        resourceCount += amount;
        UpdateResourceCountUI();

        Debug.Log($"{resourceType} count increased by {amount}. Total {resourceType} count: {resourceCount}");
    }
    private void UpdateResourceCountUI()
    {
        resourceCountText.text = $"{resourceType}: {resourceCount}";
    }

    // Q: Where can i put this script?
    // A: You can create an empty GameObject in your scene and attach the Resource script to it. Then, you can assign the appropriate values for the resourceType and resourceCount fields in the Inspector. You can also assign a Text component to the resourceCountText field to display the resource count in the UI.
    // Q: How to create a text componet
    // Q: I have a Text object but dont know what type is it?
    // Q: I can only create TextMeshPro?
    // A: You can create a TextMeshPro object by right-clicking in the Hierarchy panel, selecting UI, and then TextMeshPro - Text. This will create a TextMeshPro object that you can use to display text in your game. If you prefer to use the built-in Text component, you can create a UI Text object by right-clicking in the Hierarchy panel, selecting UI, and then Text. This will create a Text object that you can use to display text in your game.
    // Q: What is the type of the TextMeshPro object?
    // A: The TextMeshPro object is of type TextMeshProUGUI, which is a component used to display text in Unity using the TextMeshPro package. You can use this component to display text with rich formatting options and improved performance compared to the built-in Text component.


    public enum ResourceType
    {
        Wood,
        Stone,
        Iron
    }
}
