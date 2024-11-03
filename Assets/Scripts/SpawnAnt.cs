using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnt : MonoBehaviour
{
    public GameObject antPrefab;
    private Vector3 spawnPosition;
    public float delay;
    public int numberOfAnts;
    

    void Start()
    {
        spawnPosition = transform.position;
        StartCoroutine(SpawnAntWithDelay());
    }

    IEnumerator SpawnAntWithDelay()
    {
        for (int i = 1; i <= numberOfAnts; i++)
        { 
            GameObject antClone = Instantiate(antPrefab, spawnPosition, Quaternion.identity);
            antClone.name = "Ant_" + i;
            yield return new WaitForSeconds(delay);
        }
    }
}
