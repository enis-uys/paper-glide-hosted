using UnityEngine;
using System;

public class SpawnItems : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemPrefabs;

    [SerializeField]
    private float spawnDelay = 1f;

    [SerializeField]
    private float spawnForce = 10f;

    [SerializeField]
    private float destroyDelay = 15f;

    private float spawnTimer = 0f;
    public GameObject markerPrefab;
    public float markerDuration = 2f;

    private Camera mainCamera;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    //make an int range between 0 and 10
    [Range(0, 10)]
    private int circleRadius = 3;

    [SerializeField]
    //this value is the offset to the border, so the items do not spawn right at the border
    private float borderOffsetValue = 3f;

    [SerializeField]
    private bool isSpawningEnabled = false;

    public static SpawnItems instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckForSpawn();
    }

    private void CheckForSpawn()
    {
        if (isSpawningEnabled)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnDelay)
            {
                SpawnItemWithDirectionAndPositionAndDestroyIt();
                spawnTimer = 0f;
            }
        }
    }

    public void ToggleSpawning(bool? enable = null)
    {
        if (enable == null)
        {
            isSpawningEnabled = !isSpawningEnabled;
            return;
        }
        else
        {
            isSpawningEnabled = enable.Value;
        }
    }

    private void SpawnItemWithDirectionAndPositionAndDestroyIt()
    {
        Vector3 spawnPosition = DetectRandomBorderPosition();
        Vector3 spawnDirection = DetectRandomDirectionTowardsPlayer(spawnPosition);

        GameObject itemPrefab = GetRandomItemPrefab();

        GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        ApplyRigigdBodyAndCollider(item, spawnDirection);
        Destroy(item, destroyDelay);
    }

    private Vector2 DetectRandomBorderPosition()
    {
        float spawnX;
        float spawnY;

        //get the orthographic extents of the camera
        float screenHeight = mainCamera.orthographicSize * 2;
        float screenWidth = screenHeight * mainCamera.aspect;
        float screenRatio = screenWidth / screenHeight;
        // define a seed value
        int seedValue = (int)DateTime.Now.Ticks;

        // initialize a random number generator with the seed value
        System.Random rand = new System.Random(seedValue);

        int randomBorder = rand.Next(0, 3);

        switch (randomBorder)
        {
            case 0: // Left border
                spawnX =
                    mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).x
                    - borderOffsetValue / screenRatio;
                spawnY = UnityEngine.Random.Range(
                    mainCamera.transform.position.y - screenHeight / 2,
                    mainCamera.transform.position.y + screenHeight / 2
                );
                break;

            case 1: // Right border

                spawnX =
                    mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f)).x
                    + borderOffsetValue / screenRatio;

                spawnY = UnityEngine.Random.Range(
                    mainCamera.transform.position.y - screenHeight / 2,
                    mainCamera.transform.position.y + screenHeight / 2
                );
                break;
            default: // Bottom border
                spawnX = UnityEngine.Random.Range(
                    mainCamera.transform.position.x - screenWidth / 2,
                    mainCamera.transform.position.x + screenWidth / 2
                );
                spawnY = mainCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)).y;
                break;
        }

        Vector2 spawnPosition = new Vector3(spawnX, spawnY);
        // Instantiate marker at the spawn position only needed for testing
        // GameObject marker = Instantiate(markerPrefab, spawnPosition, Quaternion.identity);

        // Remove the marker after the specified duration
        // Destroy(marker, markerDuration);

        return spawnPosition;
    }

    //  this method detects a random direction for the item to spawn in
    private Vector3 DetectRandomDirectionTowardsPlayer(Vector3 spawnPosition)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle.normalized; // generate a random direction vector within the circle
        Vector3 randomSpawnPosition = player.transform.position + randomDirection * circleRadius; // calculate the spawn position around the player
        Vector3 spawnDirection = randomSpawnPosition - spawnPosition; // calculate the direction vector towards the player
        spawnDirection.Normalize(); // normalize the direction vector
        return spawnDirection;
    }

    private GameObject GetRandomItemPrefab()
    {
        int index = UnityEngine.Random.Range(0, itemPrefabs.Length);

        return itemPrefabs[index];
    }

    //this method applies a rigidbody and collider to the item
    private void ApplyRigigdBodyAndCollider(GameObject item, Vector3 spawnDirection)
    {
        if (!item.TryGetComponent<Rigidbody2D>(out var itemRigidbody))
        {
            itemRigidbody = item.AddComponent<Rigidbody2D>();
        }
        if (!item.TryGetComponent<CircleCollider2D>(out var itemCollider))
        {
            itemCollider = item.AddComponent<CircleCollider2D>();
            itemCollider.isTrigger = true;
        }
        itemRigidbody.AddForce(spawnDirection * spawnForce, ForceMode2D.Impulse);
    }
}
