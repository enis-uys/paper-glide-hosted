using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float scaleIncreaseRate = 0.1f;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.R))
        {
            Debug.Log("Start");
            StartDisappear();
        }
    }

    private void StartDisappear()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
        transform.localScale += new Vector3(
            scaleIncreaseRate,
            scaleIncreaseRate,
            scaleIncreaseRate
        );

        if (transform.localScale.x >= 10f)
        {
            Destroy(gameObject);
        }
    }
}
