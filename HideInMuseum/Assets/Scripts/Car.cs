using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
	void Update ()
    {
        transform.position -= transform.right * 5.0f * Time.deltaTime;
        if ((Mathf.Abs(transform.position.x) > 60.0f) || (Mathf.Abs(transform.position.y) > 60.0f))
        {
            Destroy(gameObject);
        }
    }
}
