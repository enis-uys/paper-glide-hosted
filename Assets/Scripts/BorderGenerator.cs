using Unity.VisualScripting;
using UnityEngine;

//only call this once and copy the gameObject to the scene after load
//up or let the borders get generated automatically on run up each time
public class BorderGenerator : MonoBehaviour
{
    public float colDepth = 4f;
    public float zPosition = 0f;
    private Vector2 screenSize;
    private Transform topCollider;
    private Transform bottomCollider;
    private Transform leftCollider;
    private Transform rightCollider;
    private Vector3 cameraPos;

    private void Start()
    {
        GenerateColliders();
        GenerateRigidbodies();
    }

    private Transform AddGameObject(string name)
    {
        Transform collider = new GameObject().transform;
        collider.gameObject.layer = gameObject.layer;
        collider.name = name;
        return collider;
    }

    private void AddCollider2D(Transform collider, Vector2 localScale, Vector3 position)
    {
        BoxCollider2D boxCollider2D = collider.gameObject.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = false;
        collider.parent = transform;
        collider.localScale = localScale;
        collider.position = position;
    }

    private void AddRigidbody2D(Transform colliderTransform)
    {
        Rigidbody2D rigidbody2D = colliderTransform.gameObject.AddComponent<Rigidbody2D>();
        rigidbody2D.isKinematic = true;
        rigidbody2D.gravityScale = 0;
    }

    private void GenerateColliders()
    {
        //Generate our empty objects
        topCollider = AddGameObject("TopCollider");
        bottomCollider = AddGameObject("BottomCollider");
        rightCollider = AddGameObject("RightCollider");
        leftCollider = AddGameObject("LeftCollider");

        //Generate world space point information for position and scale calculations
        cameraPos = Camera.main.transform.position;
        screenSize.x =
            Vector2.Distance(
                Camera.main.ScreenToWorldPoint(new Vector2(0, 0)),
                Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0))
            ) * 0.5f;
        screenSize.y =
            Vector2.Distance(
                Camera.main.ScreenToWorldPoint(new Vector2(0, 0)),
                Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height))
            ) * 0.5f;

        //Change our scale and positions to match the edges of the screen...
        AddCollider2D(
            rightCollider,
            new Vector3(colDepth, screenSize.y * 2, colDepth),
            new Vector3(cameraPos.x + screenSize.x + (colDepth * 0.5f), cameraPos.y, zPosition)
        );
        AddCollider2D(
            leftCollider,
            new Vector3(colDepth, screenSize.y * 2, colDepth),
            new Vector3(cameraPos.x - screenSize.x - (colDepth * 0.5f), cameraPos.y, zPosition)
        );
        AddCollider2D(
            topCollider,
            new Vector3(screenSize.x * 2, colDepth, colDepth),
            new Vector3(cameraPos.x, cameraPos.y + screenSize.y + (colDepth * 0.5f), zPosition)
        );
        AddCollider2D(
            bottomCollider,
            new Vector3(screenSize.x * 2, colDepth, colDepth),
            new Vector3(cameraPos.x, cameraPos.y - screenSize.y - (colDepth * 0.5f), zPosition)
        );
    }

    private void GenerateRigidbodies()
    {
        if (topCollider != null)
        {
            AddRigidbody2D(topCollider);
        }

        if (bottomCollider != null)
        {
            AddRigidbody2D(bottomCollider);
        }

        if (rightCollider != null)
        {
            AddRigidbody2D(rightCollider);
        }

        if (leftCollider != null)
        {
            AddRigidbody2D(leftCollider);
        }
    }
}
