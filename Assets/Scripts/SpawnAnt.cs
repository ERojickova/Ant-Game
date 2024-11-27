using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnt : MonoBehaviour
{
    public GameObject antPrefab;
    private Vector3 spawnPosition;
    public float delay;
    public int levelId;
    private LevelData levelData;
   

    void Start()
    {
        levelData = System.Array.Find(GameManager.Instance.levelDataList.levels, level => level.levelId == levelId);
        spawnPosition = transform.position;
        StartCoroutine(SpawnAntWithDelay());
        Debug.Log("Spawn number: " +  levelData.totalAnts);
    }

    IEnumerator SpawnAntWithDelay()
    {
        for (int i = 1; i <= levelData.totalAnts; i++)
        {
            GameObject antClone = Instantiate(antPrefab, spawnPosition, Quaternion.identity);
            antClone.name = "Ant_" + i;
            yield return new WaitForSeconds(delay);
        }
    }
}
