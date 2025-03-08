using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceType resourceType;
    public int resourceCount = 0;
    public TextMeshProUGUI resourceCountText;

    private void Start()
    {
        UpdateResourceCountUI();
    }

    public void AddResource(int amount)
    {
        resourceCount += amount;
        UpdateResourceCountUI();

        //Debug.Log($"{resourceType} count increased by {amount}. Total {resourceType} count: {resourceCount}");
    }
    private void UpdateResourceCountUI()
    {
        resourceCountText.text = $"{resourceType}: {resourceCount}";
    }

    public enum ResourceType
    {
        Wood,
        Stone
    }
}
