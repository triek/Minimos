using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class FlowerSpawnData
    {
        public GameObject prefab;
        public float spawnRate = 1f; // Relative chance for this flower
    }

    [Header("Spawner Settings")]
    public List<FlowerSpawnData> flowerSpawnDataList = new();
    public int numberOfFlowers = 5;      // Number of flowers to spawn for batch
    public float spawnDelay = 0f;        // Delay between spawns

    private Camera mainCamera;
    private bool isSpawning = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Call this from "BlueAdd1" button
    public void SpawnOneFlower()
    {
        if (!isSpawning)
            SpawnFlower();
    }

    // Call this from "BlueAdd5" button
    public void SpawnFiveFlowers()
    {
        if (!isSpawning)
            StartCoroutine(SpawnFlowers(numberOfFlowers));
    }

    private IEnumerator SpawnFlowers(int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            SpawnFlower();
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    private void SpawnFlower()
    {
        if (flowerSpawnDataList.Count == 0)
            return;

        GameObject prefab = GetRandomFlowerPrefab();
        if (prefab == null)
            return;

        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject flower = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Randomly flip horizontally if SpriteRenderer exists
        SpriteRenderer sr = flower.GetComponent<SpriteRenderer>();
        if (sr != null && Random.value > 0.5f)
        {
            sr.flipX = true;
        }
    }

    private GameObject GetRandomFlowerPrefab()
    {
        // Weighted random selection based on spawnRate
        float total = 0f;
        foreach (var data in flowerSpawnDataList)
            total += data.spawnRate;

        float rand = Random.Range(0, total);
        float cumulative = 0f;
        foreach (var data in flowerSpawnDataList)
        {
            cumulative += data.spawnRate;
            if (rand <= cumulative)
                return data.prefab;
        }
        // Fallback
        return flowerSpawnDataList[0].prefab;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 screenCenter = mainCamera.transform.position;
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        float x = Random.Range(screenCenter.x - screenWidth / 2, screenCenter.x + screenWidth / 2);
        float y = Random.Range(screenCenter.y - screenHeight / 2, screenCenter.y + screenHeight / 2);

        return new Vector3(x, y, 0);
    }
}
