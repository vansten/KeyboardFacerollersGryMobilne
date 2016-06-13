using UnityEngine;
using System.Collections;

public class Bus : MonoBehaviour
{
    private float _stopTime;
    private bool _stop = false;

    void Start ()
    {
        transform.Rotate(Vector3.forward * 180.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(_stop)
        {
            if (Time.time - _stopTime > 5.0f)
            {
                _stop = false;
            }
        }
        else
        {
            transform.position += transform.right * Time.deltaTime * 3.0f;
            if ((Mathf.Abs(transform.position.x) > 60.0f) || (Mathf.Abs(transform.position.y) > 60.0f))
            {
                Destroy(gameObject);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("BusStop"))
        {
            _stop = true;
            _stopTime = Time.time;
        }
    }
}
