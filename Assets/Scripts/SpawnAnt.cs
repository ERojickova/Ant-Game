using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnt : MonoBehaviour
{
    public GameObject antPrefab;
    private Vector3 spawnPosition;
    public Quaternion spawnRotation;
    public float delay;
    public int numberOfAnts;
    

    void Start()
    {
        spawnPosition = transform.position;
        StartCoroutine(SpawnAntWithDelay());
    }

    IEnumerator SpawnAntWithDelay()
    {
        for (int i = 1; i < numberOfAnts; i++)
        {
            yield return new WaitForSeconds(delay);
            GameObject antClone = Instantiate(antPrefab, spawnPosition, spawnRotation);
            antClone.name = "Ant_" + i;
        }
    }
}
