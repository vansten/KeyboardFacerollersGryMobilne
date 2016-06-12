using UnityEngine;
using System.Collections;

public class Carousel : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.forward * 90.0f * Time.deltaTime);
    }
}
