using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiggingDown : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isActivated = false;
    private float antLenght = 1.2f;
    private List<PolygonCollider2D> collidersToShrinkInY = new();
    private List<PolygonCollider2D> collidersToRemove = new();
    private List<GameObject> holesToGrow = new();
    public float diggingSpeed = 0.001f;
    public GameObject holeMaskPrefab;

    private AntMovement antMovementScript;
    void Start()
    {
        antMovementScript = GetComponent<AntMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && antMovementScript.role != -1)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.name == gameObject.name)
            {
                antMovementScript.speed = 0f;
                isActivated = true;
                antMovementScript.role = -1;
            }
            
        }

        // Shrinking collider:
        foreach (PolygonCollider2D collider2D in collidersToShrinkInY) {
            Vector2[] newPoints = collider2D.points;
            newPoints[1].y -= (diggingSpeed * Time.deltaTime);
            newPoints[2].y -= (diggingSpeed * Time.deltaTime);

            collider2D.points = newPoints;

            if (newPoints[1].y <= newPoints[0].y) {
                // collider2D.enabled = false;
                // Destroy(collider2D);
                collidersToRemove.Add(collider2D);
                antMovementScript.role = 0;
                antMovementScript.speed = 2f;
            }
        }

        foreach (GameObject hole in holesToGrow)
        {
            Vector3 oldSize = hole.transform.localScale;
            oldSize.y += ((diggingSpeed * 3.1f) * Time.deltaTime);
            hole.transform.localScale = oldSize;

            Vector3 oldPosition = hole.transform.position;
            oldPosition.y -= ((diggingSpeed * 1.55f) * Time.deltaTime);
            hole.transform.position = oldPosition;
        }
        

        foreach (PolygonCollider2D collider2D in collidersToRemove)
        {
            Destroy(collider2D);
            collidersToShrinkInY.Remove(collider2D);
        }
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && isActivated)
        {
            isActivated = false;
            Debug.Log("Starting digging");
            PolygonCollider2D polygonCollider2D = collision.collider.GetComponent<PolygonCollider2D>();
            StartDiggingDown(polygonCollider2D, collision.gameObject);
        }
    }

    void StartDiggingDown(PolygonCollider2D pcLeft, GameObject ground)
    {
        Debug.Log(ground.name);
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
        Vector3 newHolePosition = transform.position;
        newHolePosition.y += 0.3f;
        GameObject newHole = Instantiate(holeMaskPrefab, newHolePosition, Quaternion.identity);

        collidersToShrinkInY.Add(pcMiddle);
        holesToGrow.Add(newHole);
    }
}
