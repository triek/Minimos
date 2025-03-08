using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSpawnData
    {
        public GameObject prefab;
        public float spawnRate = 3f; // Customize for each resource type if needed
    }

    [SerializeField] private List<ResourceSpawnData> resourceSpawnDataList = new();
    [SerializeField] private Vector2 spawnArea = new Vector2(22, 7);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float spawnDistance = 0.3f;

    private void Start()
    {
        GenerateResources();
    }

    private void GenerateResources()
    {
        if (resourceSpawnDataList.Count <= 0)
        {
            return;
        }

        for (float x = 0; x < spawnArea.x; x += spawnDistance)
        {
            var actualX = Mathf.Round(x - spawnArea.x / 2);

            for (float y = 0; y < spawnArea.y; y += spawnDistance)
            {
                var actualY = Mathf.Round(y - spawnArea.y / 2);

                var spawnPosition = (Vector2)transform.position + new Vector2(actualX, actualY);

                // Skip if there's something already there
                if (Physics2D.OverlapCircle(spawnPosition, spawnDistance / 2, groundLayer))
                {
                    continue;
                }

                TrySpawnResourceAtPosition(spawnPosition);
            }
        }
    }

    private void TrySpawnResourceAtPosition(Vector2 position)
    {
        foreach (var resourceData in resourceSpawnDataList)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= resourceData.spawnRate)
            {
                Instantiate(resourceData.prefab, position, Quaternion.identity, transform);
                return; // Spawn only one resource per position
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}
