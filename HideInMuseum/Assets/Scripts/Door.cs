using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Transform _leftPart;
    [SerializeField]
    private Transform _rightPart;

    private bool _isOpened = false;

    public void Open()
    {
        if(_isOpened)
        {
            return;
        }

        _isOpened = true;
        _leftPart.Rotate(Vector3.forward, 90.0f);
        _rightPart.Rotate(Vector3.forward, -90.0f);
    }

    public void Close()
    {
        if (!_isOpened)
        {
            return;
        }

        _isOpened = false;
        _leftPart.Rotate(Vector3.forward, -90.0f);
        _rightPart.Rotate(Vector3.forward, 90.0f);
    }
}
