using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiggingDown : MonoBehaviour
{

    private class ColliderMaskPair
    {
        public PolygonCollider2D collider;
        public GameObject mask;

        public ColliderMaskPair(PolygonCollider2D col, GameObject msk)
        {
            collider = col;
            mask = msk;
        }
    }


    private bool isActivated = false;
    private float antLenght = 1.8f;
    private List<ColliderMaskPair> colMskList = new();
    private List<ColliderMaskPair> colMskListToRemove = new();
    public float diggingSpeedWorld = 0.3f;
    public GameObject holeMaskPrefab;

    private AntMovement antMovementScript;
    void Start()
    {
        antMovementScript = GetComponent<AntMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && antMovementScript.role == 0)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.name == gameObject.name)
            {
                antMovementScript.speed = 0f;
                isActivated = true;
                antMovementScript.role = 1;
            }
        }

        ShrinkCollider();
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && isActivated)
        {
            isActivated = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Collider2D colliderThatCollided = contact.collider;
            PolygonCollider2D[] allPolygons = colliderThatCollided.GetComponents<PolygonCollider2D>();
        }

            PolygonCollider2D polygonCollider2D = collision.collider.GetComponent<PolygonCollider2D>();
            Debug.Log(polygonCollider2D.points);
            StartDiggingDown(polygonCollider2D, collision.gameObject);
        }
    }

    void StartDiggingDown(PolygonCollider2D collider, GameObject ground)
    {
        if (ground.name == "MiddleCollider")
        {
            antMovementScript.speed = 2f;
            antMovementScript.role = 0;
            return;
        }
        // Debug.Log("In StartDiggingown function");
        // printCollider(pcLeft);
        // Debug.Log(ground.name);

        GameObject rightColliderObject = makeChild(ground, "RightCollider");
        GameObject leftColliderObject = makeChild(ground, "LeftCollider");
        GameObject middleColliderObject = makeChild(ground, "MiddleCollider");

        
        PolygonCollider2D pcRight = rightColliderObject.GetComponent<PolygonCollider2D>();
        PolygonCollider2D pcLeft = leftColliderObject.GetComponent<PolygonCollider2D>();
        PolygonCollider2D pcMiddle = middleColliderObject.GetComponent<PolygonCollider2D>();
        
        /*Vector3[] colliderWorldPoints = new Vector3[4];
        Vector2[] pcRightLocalPoints = new Vector2[4];

        for (int i = 0; i < 4; i++)
        {
            colliderWorldPoints[i] = collider.transform.TransformPoint(colliderWorldPoints[i]);
            Debug.Log("world coord point: " + colliderWorldPoints[i]);
        }

        for (int i = 0; i < 4;  ++i)
        {
            pcRightLocalPoints[i] = collider.transform.InverseTransformPoint(colliderWorldPoints[i]);
            Debug.Log("loacl coord point: " + pcRightLocalPoints[i]);
        }

        pcRight.points = pcRightLocalPoints;
        pcLeft.points = collider.points;
        pcMiddle.points = collider.points;*/

        Destroy(collider);

        Vector3 playerLeftWorldPosition = transform.position;
        Vector3 playerRightWorldPosition = transform.position;

        playerLeftWorldPosition.x -= (antLenght/2);
        playerRightWorldPosition.x += (antLenght/2);

        Vector3 playerLeftLocalPosition = pcLeft.transform.InverseTransformPoint(playerLeftWorldPosition);
        Vector3 playerRightLocalPosition = pcRight.transform.InverseTransformPoint(playerRightWorldPosition);

        Vector2[] newPointsLeft = pcLeft.points;
        Vector2[] newPointsRight = pcRight.points;

        // newPointsRight[2].x = newPointsLeft[2].x;
        // newPointsRight[3].x = newPointsLeft[3].x;

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
        
        if (newPointsMiddle[0].x < newPointsRight[3].x)
        {
            Debug.Log("middle[0]: " + pcMiddle.transform.TransformPoint(newPointsMiddle[0]).x);
            Debug.Log("right[3]" + pcRight.transform.TransformPoint(newPointsRight[3]).x);

            newPointsMiddle[0] = newPointsRight[3];
            newPointsMiddle[1] = newPointsRight[2];

            // Destroy(pcRight);
            // Destroy(rightColliderObject);
        }

        if (newPointsMiddle[3].x > newPointsLeft[0].x)
        {
            newPointsMiddle[3] = newPointsLeft[0];
            newPointsMiddle[2] = newPointsLeft[1];  
            // Destroy(pcLeft);
            // Destroy(leftColliderObject);
        }
     
        // PolygonCollider2D pcMiddle = ground.AddComponent<PolygonCollider2D>();
        pcMiddle.points = newPointsMiddle;
        Vector3 newHolePosition = transform.position;
        newHolePosition.y += 0.3f;
        GameObject newHole = Instantiate(holeMaskPrefab, newHolePosition, Quaternion.identity);
        Vector3 oldScale = newHole.transform.localScale;
        oldScale.x = antLenght;
        newHole.transform.localScale = oldScale;
        
        colMskList.Add(new ColliderMaskPair(pcMiddle, newHole));
    }

    void ShrinkCollider()
    {
        foreach (ColliderMaskPair pair in colMskList)
        {
            PolygonCollider2D collider2D = pair.collider;
            GameObject hole = pair.mask;

            Vector2[] newPoints = collider2D.points;

            Vector3 newPointWorld1 = collider2D.transform.TransformPoint(newPoints[1]);
            Vector3 newPointWorld2 = collider2D.transform.TransformPoint(newPoints[2]);
            newPointWorld1.y -= (diggingSpeedWorld * Time.deltaTime);
            newPointWorld2.y -= (diggingSpeedWorld * Time.deltaTime);

            newPoints[1] = collider2D.transform.InverseTransformPoint(newPointWorld1);
            newPoints[2] = collider2D.transform.InverseTransformPoint(newPointWorld2);

            collider2D.points = newPoints;


            Vector3 oldSize = hole.transform.localScale;
            oldSize.y += ((diggingSpeedWorld) * Time.deltaTime);
            hole.transform.localScale = oldSize;

            Vector3 oldPosition = hole.transform.position;
            oldPosition.y -= ((diggingSpeedWorld * 0.5f) * Time.deltaTime);
            hole.transform.position = oldPosition;

            if (newPoints[1].y <= newPoints[0].y) {
                colMskListToRemove.Add(pair);
                // collidersToRemove.Add(collider2D);
                antMovementScript.speed = 2f;
                antMovementScript.role = 0;
            }
        }        

        foreach (ColliderMaskPair pair in colMskListToRemove)
        {
            Destroy(pair.collider);
            colMskList.Remove(pair);
        }
    }

    GameObject makeChild(GameObject parent, string childName)
    {
        GameObject child = Instantiate(parent, Vector3.zero, Quaternion.identity);
        child.name = childName;
        child.transform.SetParent(parent.transform, false);

        SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
        

        for (int i = child.transform.childCount - 1; i >= 0; i--)
        {
            GameObject grandchild = child.transform.GetChild(i).gameObject;
            Destroy(grandchild);
        }

        PolygonCollider2D originalCollider = parent.GetComponent<PolygonCollider2D>();
        PolygonCollider2D childCollider = child.GetComponent<PolygonCollider2D>();
        Vector2[] localPoints = new Vector2[originalCollider.points.Length];

        for (int i = 0; i < originalCollider.points.Length; i++)
        {
            localPoints[i] = child.transform.InverseTransformPoint(parent.transform.TransformPoint(originalCollider.points[i]));
        }

        childCollider.points = localPoints;

        return child;
    }
}



