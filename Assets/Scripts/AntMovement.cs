using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    public float speed = 2f;
    public bool isFacingRight = false;
    public AntRole role = 0; // -1 = in_progress, 0 = walking, 1 = mining down
    private List<PolygonCollider2D> collidersToShrinkInY = new();
    private List<PolygonCollider2D> collidersToRemove = new();
    public float startFallingY;
    private float fallDamageThreshold = 7f;
    private bool isClimbing = false;
    public bool isDiggingSide = false;
    private bool hasBeenFliped = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>(); 
    }


    void Update()
    {
        Vector3 movement_vector = Vector3.left;
        if (isFacingRight)
        {
            movement_vector = Vector3.right;
        }

        if (isClimbing)
        {
            movement_vector = Vector3.up;
        }
        
        transform.Translate(movement_vector * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if ((startFallingY - transform.position.y > fallDamageThreshold) && (role != AntRole.Floater))
        {
            Destroy(gameObject);
        }

        if ((collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Wall") && role != AntRole.Climber && !isDiggingSide && !hasBeenFliped)
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

        if (collision.gameObject.tag == "Wall" && role == AntRole.Climber)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 normal = contact.normal;
                if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
                {
                    isClimbing = true;
                    rb.gravityScale = 0.0f;
                }
            }
        }

    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            startFallingY = transform.position.y;
        }

        if (collision.gameObject.tag == "Wall")
        {
            isClimbing = false;
            rb.gravityScale = 1.0f;
        }

        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Obstacle")
        {
            hasBeenFliped = false;
        }
        
    }

    void Flip()
    {
        hasBeenFliped = true;
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
