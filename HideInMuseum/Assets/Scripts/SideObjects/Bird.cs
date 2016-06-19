using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
    private static Vector3[] _corners = new Vector3[4]
    {
        new Vector3(-1000.0f, -1000.0f, 0.0f),
        new Vector3(-1000.0f, 1000.0f, 0.0f),
        new Vector3(1000.0f, -1000.0f, 0.0f),
        new Vector3(1000.0f, 1000.0f, 0.0f)
    };
    private Vector3 _direction;

    void OnEnable()
    {
        Vector3 target = Vector3.zero;
        float minDist = float.MaxValue;

        for(int i = 0; i < _corners.Length; ++i)
        {
            float dist = Vector3.Distance(transform.position, _corners[i]);
            if(dist < minDist)
            {
                minDist = dist;
                target = _corners[i];
            }
        }

        _direction = target - transform.position;
        _direction.Normalize();
        _direction = Quaternion.Euler(0.0f, 0.0f, Random.Range(-30.0f, 30.0f)) * _direction;
        transform.up = _direction;
    }

    void Update()
    {
        transform.position += _direction * 5.0f * Time.deltaTime;

        if(Mathf.Abs(transform.position.x) > 100.0f || Mathf.Abs(transform.position.y) > 100.0f)
        {
            Destroy(gameObject);
        }
    }
}
