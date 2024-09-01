using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMovement : MonoBehaviour
{
    private float speed = 2f;
    private bool isFacingRight = false;
    private int role = 0; // -1 = in_progress, 0 = walking, 1 = mining down
    private float antLenght = 1.2f;
    private List<PolygonCollider2D> collidersToShrinkInY = new();

    public float diggingSpeed = 0.0001f;

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

        if (Input.GetMouseButtonDown(0)) // left mouse button
        {
            if (role != 1) {
                role = 1;
            } else {
                role = 0;
                speed = 2f;
            }
        }

        // Shrinking collider:
        foreach (PolygonCollider2D collider2D in collidersToShrinkInY) {
            Vector2[] newPoints = collider2D.points;
            newPoints[1].y -= (diggingSpeed * Time.deltaTime);
            newPoints[2].y -= (diggingSpeed * Time.deltaTime);

            collider2D.points = newPoints;

            if (newPoints[1].y <= newPoints[0].y) {
                collider2D.enabled = false;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && role == 1)
        {
            role = -1;
            speed = 0.0f;
            Debug.Log("Player is standing on: " + collision.gameObject.name);
            Debug.Log("Player's position: " + transform.position);
            PolygonCollider2D polygonCollider2D = collision.collider.GetComponent<PolygonCollider2D>();
            // Debug.Log("PolygonCollider2D points:");
            // for (int i = 0; i < polygonCollider2D.points.Length; i++)
            // {
            //     Debug.Log("Point " + i + ": " + polygonCollider2D.points[i]);
            // }
            
            // Vector3 playerWorldPosition = transform.position;
            // Vector3 playerLocalPosition = polygonCollider2D.transform.InverseTransformPoint(playerWorldPosition);
            // Debug.Log("Player Local Position relative to PolygonCollider2D: " + playerLocalPosition);

            StartDiggingDown(polygonCollider2D, collision.gameObject);
        }
    }

    void StartDiggingDown(PolygonCollider2D pcLeft, GameObject ground)
    {
        PolygonCollider2D pcRight = ground.AddComponent<PolygonCollider2D>();

        Vector3 playerLeftWorldPosition = transform.position;
        Vector3 playerRightWorldPosition = transform.position;

        playerLeftWorldPosition.x -= (antLenght/2);
        playerRightWorldPosition.x += (antLenght/2);

        Vector3 playerLeftLocalPosition = pcLeft.transform.InverseTransformPoint(playerLeftWorldPosition);
        Vector3 playerRightLocalPosition = pcRight.transform.InverseTransformPoint(playerRightWorldPosition);

        Vector2[] newPointsLeft = pcLeft.points;
        Vector2[] newPointsRight = pcRight.points;

        newPointsLeft[2].x = playerLeftLocalPosition.x;
        newPointsLeft[3].x = playerLeftLocalPosition.x;

        newPointsRight[0].x = playerRightLocalPosition.x;
        newPointsRight[1].x = playerRightLocalPosition.x;

        pcLeft.points = newPointsLeft;
        pcRight.points = newPointsRight;

        // Adding middle collider
        Vector2[] newPointsMiddle = new Vector2[]
        {
            newPointsLeft[2],
            newPointsLeft[3],
            newPointsRight[0],
            newPointsRight[1]
            
        };

        PolygonCollider2D pcMiddle = ground.AddComponent<PolygonCollider2D>();
        pcMiddle.points = newPointsMiddle;

        collidersToShrinkInY.Add(pcMiddle);
    }

    

    Vector2 AbsoluteToLocal(Vector2 absolutePosition, GameObject gameObject)
    {
        return (Vector2)gameObject.transform.InverseTransformPoint(absolutePosition);
    }
    
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 200), "Role: " + role);
    }
}
