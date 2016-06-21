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

public class Room : ObjectBase
{
    public RoomType Type;
    public GameObject Darkness;
    public GameObject Water;
    public GameObject DarknessIcon;
    public GameObject WaterIcon;
    public GameObject LockedObejct;
    [HideInInspector]
    public bool IsQTEActive;
    [HideInInspector]
    public bool Unlocked;

    private float _timer;
    private float _cooldown;

    public override void OnEnable()
    {
        base.OnEnable();
        _timer = 0.0f;
        _cooldown = Random.Range(QTEController.Instance.CooldownRange.x, QTEController.Instance.CooldownRange.y);
        Darkness.SetActive(false);
        DarknessIcon.SetActive(false);
        Water.SetActive(false);
        WaterIcon.SetActive(false);
        IsQTEActive = false;

        LockedObejct.SetActive(!Unlocked);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > _cooldown)
        {
            QTEController.Instance.SpawnQTE(this);
            _timer = 0.0f;
            _cooldown = Random.Range(QTEController.Instance.CooldownRange.x, QTEController.Instance.CooldownRange.y);
         }
    }

    public override void OnStatisticsWindowBegin()
    {
        Darkness.SetActive(false);
        DarknessIcon.SetActive(false);
        Water.SetActive(false);
        WaterIcon.SetActive(false);
        IsQTEActive = false;
        enabled = false;
    }

    public override void OnVisitStageBegin()
    {
        enabled = true;
    }

    public override void OnMenuBegin()
    {
        Darkness.SetActive(false);
        DarknessIcon.SetActive(false);
        Water.SetActive(false);
        WaterIcon.SetActive(false);
        IsQTEActive = false;
        enabled = false;
    }
}
