using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class GravityObject : MonoBehaviour
{
    [SerializeField]
    private float gravityScale = 1f;

    [SerializeField]
    private float radius = 1f;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        rb.gravityScale = gravityScale;
        circleCollider.radius = radius;
    }
}
