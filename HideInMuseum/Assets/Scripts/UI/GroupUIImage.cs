using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupUIImage : MonoBehaviour
{
    public Image SelectedImage;
    public Image DangerImage;
    public Image GroupImage;

    private GroupMovement _myGroup;
    private float _timer;
    private bool _rise;

    public void Initialize(GroupMovement group)
    {
        _myGroup = group;
        gameObject.SetActive(_myGroup != null);
        if(_myGroup != null)
        {
            GroupImage.color = _myGroup.MyColor;
        }
        DangerImage.enabled = false;
    }

    public void UpdateUI()
    {
        if(InputHandler.Instance.CurrentGroup == _myGroup && !SelectedImage.enabled)
        {
            SelectedImage.enabled = true;
        }
        else if(InputHandler.Instance.CurrentGroup != _myGroup && SelectedImage.enabled)
        {
            SelectedImage.enabled = false;
        }

        if(_myGroup != null)
        {
            DangerImage.enabled = _myGroup.IsSad;
        }

        if (DangerImage.enabled)
        {
            if(_rise)
            {
                _timer += Time.deltaTime * 4.0f;
                if(_timer > 1.0f)
                {
                    _rise = false;
                }
            }
            else
            {
                _timer -= Time.deltaTime * 4.0f;
                if(_timer < 0.0f)
                {
                    _rise = true;
                }
            }

            DangerImage.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, _timer);
        }
        else
        {
            _timer = 0.0f;
            _rise = true;
            DangerImage.transform.localScale = Vector3.one;
        }
    }
}
