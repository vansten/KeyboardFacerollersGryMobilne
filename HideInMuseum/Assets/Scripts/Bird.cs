using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
    private Vector3 _direction;

    void OnEnable()
    {
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(-60.0f, -60.0f, 0.0f);
        corners[1] = new Vector3(-60.0f, 60.0f, 0.0f);
        corners[2] = new Vector3(60.0f, -60.0f, 0.0f);
        corners[3] = new Vector3(60.0f, 60.0f, 0.0f);

        Vector3 target = Vector3.zero;
        float minDist = float.MaxValue;

        for(int i = 0; i < 4; ++i)
        {
            float dist = Vector3.Distance(transform.position, corners[i]);
            if(dist < minDist)
            {
                minDist = dist;
                target = corners[i];
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
