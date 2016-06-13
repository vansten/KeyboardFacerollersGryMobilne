using UnityEngine;
using System.Collections;

public class Bus : MonoBehaviour
{
    private float _stopTime;
    private float _stopTimer = 0.0f;
    private float _spawnCooldown = 0.0f;
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
            _stopTimer += Time.deltaTime;

            if(_stopTimer > _spawnCooldown)
            {
                SpawnPassenger();
            }

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

    void SpawnPassenger()
    {
        _stopTimer = 0.0f;
        _spawnCooldown = Random.Range(0.5f, 1.5f);
        GameObject go = PeopleManager.Instance.GetRandomPerson();
        Vector3 position = transform.position;
        position.x += Random.Range(-1.0f, 1.0f);
        position.y += 0.3f;
        GameObject passenger = (GameObject)Instantiate(go, position, Quaternion.Euler(0.0f, 0.0f, 180.0f));
        passenger.AddComponent<BusPassenger>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("BusStop"))
        {
            _stop = true;
            _stopTimer = 0.0f;
            _spawnCooldown = Random.Range(0.5f, 1.5f);
            _stopTime = Time.time;
        }
    }
}
