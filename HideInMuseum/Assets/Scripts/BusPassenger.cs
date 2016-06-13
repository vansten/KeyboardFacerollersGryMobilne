using UnityEngine;
using System.Collections;

public class BusPassenger : MonoBehaviour
{
    private Vector3 _initPosition;
    private Vector3 _targetPosition;
    private Vector3 _initRotation;
    private Vector3 _targetRotation;
    private float _timer;

    void OnEnable()
    {
        _timer = 0.0f;
        _initPosition = transform.position;
        _targetPosition = transform.position - transform.up * 2.0f;
        _initRotation = transform.rotation.eulerAngles;
        _targetRotation = _initRotation;
        _targetRotation.z += ((Random.Range(0, 2) % 2) * 2 - 1) * 90.0f;
    }

    void Update()
    {
        if (_timer <= 1.0f)
        {
            transform.position = Vector3.Lerp(_initPosition, _targetPosition, _timer);
        }
        else if(_timer <= 2.0f)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(_initRotation, _targetRotation, _timer - 1.0f));
        }
        else
        {
            PeopleManager.Instance.AddPasserby(gameObject);
            enabled = false;
        }

        _timer += Time.deltaTime * 2.0f;
    }
}
