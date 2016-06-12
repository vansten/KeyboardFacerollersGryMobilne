using UnityEngine;
using System.Collections;

public enum RoomType
{
    OldCameras = 0,
    CostumesAndSuits,
    Posters,
    Tapes,
    Animations, 
    Characters,
    Entrance
}

public class Room : MonoBehaviour
{
    public RoomType Type;
    public GameObject Darkness;
    public GameObject Water;
    public GameObject DarknessIcon;
    public GameObject WaterIcon;
    [HideInInspector]
    public bool IsQTEActive;

    private float _timer;
    private float _cooldown;

    void OnEnable()
    {
        _timer = 0.0f;
        _cooldown = Random.Range(QTEController.Instance.CooldownRange.x, QTEController.Instance.CooldownRange.y);
        IsQTEActive = false;
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _cooldown)
        {
            QTEController.Instance.SpawnQTE(this);
            _timer = 0.0f;
            _cooldown = Random.Range(QTEController.Instance.CooldownRange.x, QTEController.Instance.CooldownRange.y);
        }
    }
}
