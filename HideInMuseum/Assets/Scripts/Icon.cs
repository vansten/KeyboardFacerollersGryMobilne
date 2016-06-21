using UnityEngine;
using System.Collections;

public class Icon : ObjectBase
{
    public Room MyRoom;

    private SpriteRenderer _spriteRenderer;
    private Color c = Color.white;
    private float _lastTapTime = 0.0f;
    private float _showTime;
    private int _tapCount;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _showTime = Time.time;
    }

    void Update()
    {
        if(Time.time - _lastTapTime > 1.0f)
        {
            _tapCount = 0;
        }

        if(_tapCount >= 5)
        {
            QTEController.Instance.RemoveQTE(MyRoom);
        }

        float sin = Mathf.Sin((Time.time - _showTime) * 5.0f);
        c.a = Mathf.Lerp(0.0f, 1.0f, sin * sin);
        _spriteRenderer.color = c;
    }

    public void Tap()
    {
        _lastTapTime = Time.time;
        _tapCount += 1;
    }

    public override void OnStatisticsWindowBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnMenuBegin()
    {
        gameObject.SetActive(false);
    }
}
