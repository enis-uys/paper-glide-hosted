using UnityEngine;
using System;

[RequireComponent(typeof(Transform))]
public class MoveWithPlayer : MonoBehaviour
{
    public Transform playerTransform;

    private float initialY;
    private float offset;

    private void Start()
    {
        initialY = transform.position.y;
        offset = transform.position.x - playerTransform.position.x;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(
            playerTransform.position.x + offset,
            initialY,
            transform.position.z
        );
        transform.position = newPosition;
    }
}
