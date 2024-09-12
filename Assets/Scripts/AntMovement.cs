using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    public float speed = 2f;
    private bool isFacingRight = false;
    public int role = 0; // -1 = in_progress, 0 = walking, 1 = mining down
    private List<PolygonCollider2D> collidersToShrinkInY = new();
    private List<PolygonCollider2D> collidersToRemove = new();
    public float startFallingY;
    private float fallDamageThreshold = 7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }


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
        if (startFallingY - transform.position.y > fallDamageThreshold)
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Obstacle")
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;
                if (normal == Vector2.left || normal == Vector2.right) {
                    Flip();
                    break;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        startFallingY = transform.position.y;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
