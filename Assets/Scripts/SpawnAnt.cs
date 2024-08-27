using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnt : MonoBehaviour
{
    public GameObject antPrefab;
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public float delay;
    public int numberOfAnts;
    

    void Start()
    {
        StartCoroutine(SpawnAntWithDelay());
    }

    IEnumerator SpawnAntWithDelay()
    {
        for (int i = 0; i < numberOfAnts - 1; i++)
        {
            yield return new WaitForSeconds(delay);
            Instantiate(antPrefab, spawnPosition, spawnRotation);
        }
    }
}