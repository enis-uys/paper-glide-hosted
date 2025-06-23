using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxBackground : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    [SerializeField]
    private Vector2 parallaxMultiplier;

    [Tooltip(
        "Makes it possible to improve the borders for changing the background position for the infinite effect so you do not see empty space at the borders."
    )]
    [SerializeField]
    [Range(-20f, 20f)]
    private float cameraDistanceImprove;

    [Tooltip("This variable is needed for changing the paralax layer speed.")]
    [SerializeField]
    [Range(0f, 1f)]
    private float parallaxLayerMultiplier;
    private Dictionary<GameObject, float> textureUnitSizeDict;

    private GameObject[] nearChildrenBackgrounds;

    private void Start()
    {
        Init();
    }

    void LateUpdate()
    {
        UpdateBackgrounds();
    }

    private void Init()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        // Get all child GameObjects of "Near" in the hierarchy
        GameObject nearGameObject = gameObject;

        int childCount = gameObject.transform.childCount;
        nearChildrenBackgrounds = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            nearChildrenBackgrounds[i] = gameObject.transform.GetChild(i).gameObject;
        }
        // Initialize the dictionary
        textureUnitSizeDict = new Dictionary<GameObject, float>();
        // Loop through each game object in the array
        if (nearChildrenBackgrounds != null)
        {
            foreach (GameObject obj in nearChildrenBackgrounds)
            {
                // Get the sprite renderer component and sprite from the game object
                SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                Sprite sprite = spriteRenderer.sprite;

                // Calculate the texture unit size for the sprite
                float textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;

                // Add the game object and texture unit size to the dictionary
                textureUnitSizeDict.Add(obj, textureUnitSizeX);
            }
        }
    }

    private void UpdateBackgrounds()
    {
        Vector2 tempParallaxMultiplier = parallaxMultiplier;
        for (int i = 0; i < nearChildrenBackgrounds.Length; i++)
        {
            // Calculate the distance the camera has moved since the last frame and moves the backgrounds by multiplyer
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            nearChildrenBackgrounds[i].transform.position += new Vector3(
                deltaMovement.x * tempParallaxMultiplier.x,
                deltaMovement.y * tempParallaxMultiplier.y
            );

            // Make infinite backgrounds by teleporting the background, that is mirrored to the left and the right size, exactly by the width of 1 tile
            foreach (KeyValuePair<GameObject, float> textureUnitSizePair in textureUnitSizeDict)
            {
                float halfCamWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
                float distanceFromBackground = Mathf.Abs(
                    cameraTransform.position.x - nearChildrenBackgrounds[i].transform.position.x
                );
                //check if cameraPosition has moved to far away from one side

                if (distanceFromBackground + halfCamWidth >= textureUnitSizePair.Value)
                {
                    float backgroundCameraDistance =
                        cameraTransform.position.x
                        - nearChildrenBackgrounds[i].transform.position.x;
                    float offsetPositionX =
                        (
                            cameraTransform.position.x
                            - nearChildrenBackgrounds[i].transform.position.x
                        ) % textureUnitSizePair.Value;
                    ;

                    if (backgroundCameraDistance > 0) // Moving to the right
                    {
                        offsetPositionX = textureUnitSizePair.Value - offsetPositionX;

                        nearChildrenBackgrounds[i].transform.position = new Vector3(
                            cameraTransform.position.x + offsetPositionX,
                            nearChildrenBackgrounds[i].transform.position.y
                        );
                    }
                    else if (backgroundCameraDistance < 0) // Moving to the left
                    {
                        nearChildrenBackgrounds[i].transform.position = new Vector3(
                            //important part, new position gets to
                            cameraTransform.position.x - offsetPositionX,
                            nearChildrenBackgrounds[i].transform.position.y
                        );
                    }
                }
            }

            tempParallaxMultiplier *= parallaxLayerMultiplier;
        }
        // Set the last camera position variable to the current camera position
        lastCameraPosition = cameraTransform.position;
    }
}
