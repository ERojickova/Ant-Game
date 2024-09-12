using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float leftBound;
    public float rightBound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime, 0, 0);   
        Vector3 oldPosition = transform.position;
        oldPosition.x = Mathf.Clamp(oldPosition.x, leftBound, rightBound);

        transform.position = oldPosition;
        
    }
}
