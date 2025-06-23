using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScriptTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;
            transform.position = touchPosition;
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch Began");
            }
            if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log("Touch Moved");
            }
            if (touch.phase == TouchPhase.Ended)
            {
                Debug.Log("Touch Ended");
            }
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector3 touchPosition2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
                Debug.DrawLine(Vector3.zero, touchPosition2, Color.red);
                Debug.Log("Touch index " + i + " is " + Input.GetTouch(i).phase);
            }
        }
    }
}
