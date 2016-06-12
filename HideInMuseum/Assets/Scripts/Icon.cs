using UnityEngine;
using System.Collections;

public class Icon : MonoBehaviour
{
    public Room MyRoom;

    private SpriteRenderer _spriteRenderer;
    private Color c = Color.white;
    private float _lastTapTime = 0.0f;
    private int _tapCount;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Time.time - _lastTapTime > 1.0f)
        {
            _tapCount = 0;
        }

        if(_tapCount > 8)
        {
            QTEController.Instance.RemoveQTE(MyRoom);
        }

        float sin = Mathf.Sin(Time.time);
        c.a = Mathf.Lerp(0.0f, 1.0f, sin * sin);
        _spriteRenderer.color = c;
    }

    public void Tap()
    {
        Debug.Log(gameObject.name);
        _lastTapTime = Time.time;
        _tapCount += 1;
    }
}
