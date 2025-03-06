using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> resourcePrefabs = new();

    [SerializeField] private Vector2 spawnArea = new Vector2(24, 8);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float spawnDistance = 0.3f;
    [SerializeField] private float spawnRate = 3f;

    private void Start()
    {
        GenerateTree();
    }

    private void GenerateTree()
    {
        if (resourcePrefabs.Count <= 0)
        {
            return;
        }

        for (float x = 0; x < spawnArea.x; x += spawnDistance)
        {
            var actualX = Mathf.Round(x - spawnArea.x / 2);

            for (float y = 0; y < spawnArea.y; y += spawnDistance)
            {
                var actualY = Mathf.Round(y - spawnArea.y / 2);

                if (UnityEngine.Random.Range(0, 100) > spawnRate)
                {
                    continue;
                }

                var spawnPosition = (Vector2)transform.position + new Vector2(actualX, actualY);

                // Check for solid objects using OverlapCircle
                if (Physics2D.OverlapCircle(spawnPosition, spawnDistance / 2, groundLayer))
                {
                    continue;
                }

                var randomResource = resourcePrefabs[UnityEngine.Random.Range(0, resourcePrefabs.Count)];
                if (randomResource == null)
                {
                    continue;
                }

                Instantiate(randomResource, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, (Vector3)spawnArea);
    }
}




