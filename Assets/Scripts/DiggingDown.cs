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
    public GameObject holeTriggerPrefab;

    

    public bool inHole = false;

    private SetRole setRoleScript;
    private AntMovement antMovementScript;

    private float printingDelay = 0; // Just for debuging 
    void Start()
    {
        antMovementScript = GetComponent<AntMovement>();
        setRoleScript = FindObjectOfType<SetRole>();
        
    }

    // Update is called once per frame
    void Update()
    {
        printingDelay += Time.deltaTime;
        if (printingDelay > 3)
        {
            printingDelay = 0;
            Debug.Log("From digging script: " + setRoleScript.activeRole + "Ant: " + transform.name);
        }

        if (Input.GetMouseButtonDown(0) && antMovementScript.role == AntRole.None && setRoleScript.activeRole == AntRole.DiggerDown)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.name == gameObject.name && !inHole)
            {
                isActivated = true;
                antMovementScript.role = AntRole.DiggerDown; 
                setRoleScript.activeRole = AntRole.None;
                setRoleScript.numDiggerDown -= 1;
                Debug.Log("Setting isActivated on true");

            }
        }

        ShrinkCollider();
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor") && isActivated)
        {
            Debug.Log("Setting isActivated on false");
            isActivated = false;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                Collider2D colliderThatCollided = contact.collider;
                PolygonCollider2D[] allPolygons = colliderThatCollided.GetComponents<PolygonCollider2D>();
            }

            PolygonCollider2D polygonCollider2D = collision.collider.GetComponent<PolygonCollider2D>();
            // Debug.Log(polygonCollider2D.points);
            StartDiggingDown(polygonCollider2D, collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            inHole = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hole"))
        {
            inHole = false;
        }
    }

    void StartDiggingDown(PolygonCollider2D collider, GameObject ground)
    {
        antMovementScript.role = AntRole.DiggerDown;
        antMovementScript.speed = 0f;
        addHat();

        Debug.Log("Start Digging");
        if (ground.name == "MiddleCollider")
        {
            antMovementScript.speed = 2f;
            antMovementScript.role = 0;
            return;
        }

        GameObject rightColliderObject = makeChild(ground, "RightCollider");
        GameObject leftColliderObject = makeChild(ground, "LeftCollider");
        GameObject middleColliderObject = makeChild(ground, "MiddleCollider");

        
        PolygonCollider2D pcRight = rightColliderObject.GetComponent<PolygonCollider2D>();
        PolygonCollider2D pcLeft = leftColliderObject.GetComponent<PolygonCollider2D>();
        PolygonCollider2D pcMiddle = middleColliderObject.GetComponent<PolygonCollider2D>();

        Destroy(collider);

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
        
        pcMiddle.points = newPointsMiddle;

        Vector3 newHolePosition = transform.position;
        newHolePosition.y += 0.3f;
        GameObject newHole = Instantiate(holeMaskPrefab, newHolePosition, Quaternion.identity);
        Vector3 oldScale = newHole.transform.localScale;
        oldScale.x = antLenght;
        newHole.transform.localScale = oldScale;

        GameObject holeTrigger = Instantiate(holeTriggerPrefab, newHolePosition, Quaternion.identity);
        Vector3 triggerScale = newHole.transform.localPosition;
        triggerScale.x = antLenght * 2;
        holeTrigger.transform.localScale = triggerScale;
        
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
                removeHat();
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

    private void printColliderPoints(PolygonCollider2D collider, string name)
    {
        Debug.Log(name + ": (" + collider.points[0].x + ", " + collider.points[0].y + "); ("
            + collider.points[1].x + ", " + collider.points[1].y + "); ("
            + collider.points[2].x + ", " + collider.points[2].y + "); ("
            + collider.points[3].x + ", " + collider.points[3].y + "); (");
    }

    private void addHat()
    {
        GameObject hat = transform.Find("digger_down_hat").gameObject;
        hat.GetComponent<SpriteRenderer>().enabled = true;

        GameObject left_antennae = transform.Find("antennae_left").gameObject;
        left_antennae.GetComponent<SpriteRenderer>().enabled = false;

        GameObject right_antennae = transform.Find("antennae_right").gameObject;
        right_antennae.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void removeHat()
    {
        GameObject hat = transform.Find("digger_down_hat").gameObject;
        hat.GetComponent<SpriteRenderer>().enabled = false;

        GameObject left_antennae = transform.Find("antennae_left").gameObject;
        left_antennae.GetComponent<SpriteRenderer>().enabled = true;

        GameObject right_antennae = transform.Find("antennae_right").gameObject;
        right_antennae.GetComponent<SpriteRenderer>().enabled = true;
    }
}



