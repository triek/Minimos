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

    private HashSet<Vector2> occupiedPositions = new(); // Keeps track of occupied spots

    private void Start()
    {
        GenerateResources();
    }

    private void GenerateResources()
    {
        if (resourceSpawnDataList.Count == 0) return;

        for (float x = 0; x < spawnArea.x; x += spawnDistance)
        {
            float actualX = Mathf.Round(x - spawnArea.x / 2);

            for (float y = 0; y < spawnArea.y; y += spawnDistance)
            {
                float actualY = Mathf.Round(y - spawnArea.y / 2);
                Vector2 spawnPosition = (Vector2)transform.position + new Vector2(actualX, actualY);

                // Skip if the position is already occupied
                if (occupiedPositions.Contains(spawnPosition) || Physics2D.OverlapCircle(spawnPosition, spawnDistance / 2, groundLayer))
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
            if (Random.Range(0f, 100f) <= resourceData.spawnRate)
            {
                GameObject spawnedResource = Instantiate(resourceData.prefab, position, Quaternion.identity, transform);

                SpriteRenderer spriteRenderer = spawnedResource.GetComponent<SpriteRenderer>();

                // Randomly flip horizontally
                if (Random.value > 0.5f && spriteRenderer != null)
                {
                    spriteRenderer.flipX = true; // Flip the sprite instead of scaling the transform
                }

                occupiedPositions.Add(position); // Mark this position as occupied
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
