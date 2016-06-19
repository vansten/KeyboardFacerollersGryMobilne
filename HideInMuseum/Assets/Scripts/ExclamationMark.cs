using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ExclamationMark : Singleton<ExclamationMark>
{
    [SerializeField]
    private Image _image;

    private List<GroupMovement> _groupsWaiting = new List<GroupMovement>();
    private Image[] _childrenOfImage;
    private Color _color;

    void Start()
    {
        _color = Color.white;
        _childrenOfImage = _image.GetComponentsInChildren<Image>().Where(i => i != _image).ToArray();
    }
    
    void Update()
    {
        if(GameManager.Instance.CurrentState != GameState.VisitStage)
        {
            _image.enabled = false;
            _color = Color.white;
            return;
        }

        _image.enabled = _groupsWaiting.Count > 0;
        foreach(Image i in _childrenOfImage)
        {
            i.enabled = _image.enabled;
        }

        if(_image.enabled)
        {
            float sin = Mathf.Sin(Time.time * 5.0f);
            _color.a = sin * sin;
            _image.color = _color;
        }
    } 

    public void Reset()
    {
        _groupsWaiting.Clear();
        _image.enabled = false;
    }

    public bool IsGroupWaiting()
    {
        return _groupsWaiting.Count > 0;
    }

    public void AddGroupWaiting(GroupMovement group)
    {
        if(!_groupsWaiting.Contains(group))
        {
            if (_groupsWaiting.Count == 0)
            {
                _color = Color.white;
            }
            _groupsWaiting.Add(group);
        }
    }

    public void RemoveGroupWaiting(GroupMovement group)
    {
        if(_groupsWaiting.Contains(group))
        {
            _groupsWaiting.Remove(group);
        }
    }

    public void ExclamationMarkClick()
    {
        if(_groupsWaiting.Count > 0)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x = _groupsWaiting[0].transform.position.x;
            cameraPos.y = _groupsWaiting[0].transform.position.y;
            Camera.main.transform.position = cameraPos;
            InputHandler.Instance.AdjustCameraTransform();
        }
    }
}
