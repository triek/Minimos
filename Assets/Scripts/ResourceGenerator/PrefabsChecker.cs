using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabChecker : MonoBehaviour
{
    [SerializeField] private List<GameObject> resourcePrefabs = new();

    private void Start()
    {
        CheckPrefabs();
    }

    private void CheckPrefabs()
    {
        if (resourcePrefabs.Count <= 0)
        {
            Debug.Log("No resources to check");
            return;
        }

        foreach (var prefab in resourcePrefabs)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                Debug.Log($"Valid prefab: {prefab.name}");
            }
            else
            {
                Debug.LogWarning($"Invalid prefab: {prefab.name}");
            }
        }
    }
}
