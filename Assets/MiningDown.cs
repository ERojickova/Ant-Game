using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningDown : MonoBehaviour
{
    private bool isMining = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse button
        {
            Mine();
        }
    }

    void Mine()
    {
        isMining = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is standing on an object tagged as "Ground"
        if (collision.gameObject.CompareTag("Obstacle") && isMining)
        {
            isMining = false;
            Debug.Log("Player is standing on: " + collision.gameObject.name);
        }
    }
}