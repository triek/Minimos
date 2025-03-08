using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject settlerPrefab; // Settler prefab to spawn
    public int numberOfSettlers = 5; // Number of settlers to spawn
    public float spawnDelay = 0.5f; // Delay between spawns
    public float spawnDistance = 2f; // Distance outside the screen to spawn

    private Camera mainCamera;
    private bool isSpawning = false; // Prevent multiple spawns at once

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isSpawning)
        {
            StartCoroutine(SpawnSettlers());
        }
    }

    private IEnumerator SpawnSettlers()
    {
        isSpawning = true;

        for (int i = 0; i < numberOfSettlers; i++)
        {
            SpawnSettler();
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
    }

    private void SpawnSettler()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject newSettler = Instantiate(settlerPrefab, spawnPos, Quaternion.identity);

        Animator animator = newSettler.GetComponent<Animator>();
        Transform settlerHead = newSettler.transform.Find("SettlerHead");

        if (animator != null && settlerHead != null)
        {
            bool hasFlower = settlerHead.childCount > 0; // Check if SettlerHead has a child
            animator.SetBool("isPickupFlower", hasFlower);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        Vector3 screenCenter = mainCamera.transform.position;
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Randomly choose a spawn side (0 = Left, 1 = Right, 2 = Top, 3 = Bottom)
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // Left
                spawnPos = new Vector3(screenCenter.x - screenWidth / 2 - spawnDistance, Random.Range(screenCenter.y - screenHeight / 2, screenCenter.y + screenHeight / 2), 0);
                break;
            case 1: // Right
                spawnPos = new Vector3(screenCenter.x + screenWidth / 2 + spawnDistance, Random.Range(screenCenter.y - screenHeight / 2, screenCenter.y + screenHeight / 2), 0);
                break;
            case 2: // Top
                spawnPos = new Vector3(Random.Range(screenCenter.x - screenWidth / 2, screenCenter.x + screenWidth / 2), screenCenter.y + screenHeight / 2 + spawnDistance, 0);
                break;
            case 3: // Bottom
                spawnPos = new Vector3(Random.Range(screenCenter.x - screenWidth / 2, screenCenter.x + screenWidth / 2), screenCenter.y - screenHeight / 2 - spawnDistance, 0);
                break;
        }

        return spawnPos;
    }
}
