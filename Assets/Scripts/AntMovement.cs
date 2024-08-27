using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    private float speed = 2f;
    private bool isFacingRight = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement_vector = Vector3.left;
        if (isFacingRight)
        {
            movement_vector = Vector3.right;
        }
        
        transform.Translate(movement_vector * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;
                Debug.Log("in colision");
                if (normal == Vector2.left || normal == Vector2.right) {
                    Debug.Log("in if");
                    Flip();
                    break;
                }
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
