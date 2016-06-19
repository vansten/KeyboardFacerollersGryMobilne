using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MenuController : ObjectBase
{
    #region Private variables

    [SerializeField]
    private GameObject _mainMenuParent;
    [SerializeField]
    private GameObject _shopMenuParent;
    [SerializeField]
    private GameObject _exitMenuParent;
    [SerializeField]
    private GameObject _aboutMenuParent;
    [SerializeField]
    private GameObject _howToPlayMenuParent;
    [SerializeField]
    private RoomImage _roomToBuy;
    [SerializeField]
    private Text _moneyText;
    [SerializeField]
    private Image _soundImage;
    [SerializeField]
    private Sprite _soundOnSprite;
    [SerializeField]
    private Sprite _soundOffSprite;
    [SerializeField]
    private Sprite _leftEndSprite;
    [SerializeField]
    private Sprite _rightEndSprite;

    #endregion

    int Comparer(RoomInfo r1, RoomInfo r2)
    {
        if (r1.Room.Unlocked == r2.Room.Unlocked)
        {
            return r1.MoneyToUnlock.CompareTo(r2.MoneyToUnlock);
        }
        else
        {
            if (r1.Room.Unlocked)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    #region Object Base methods

    public override void OnMenuBegin()
    {
        ExclamationMark.Instance.Reset();
        gameObject.SetActive(true);
        RoomInfo[] rooms = GameManager.Instance.Rooms.ToArray();
        Array.Sort(rooms, Comparer);
        _roomToBuy.SetRoom(rooms[0]);
        RectTransform firstTransform = _roomToBuy.GetComponent<RectTransform>();
        for(int i = 1; i < rooms.Length; ++i)
        {
            GameObject go = Instantiate(_roomToBuy.gameObject);
            go.GetComponent<RoomImage>().SetRoom(rooms[i]);
            go.transform.SetParent(firstTransform.parent, false);
            Vector3 position = firstTransform.anchoredPosition + firstTransform.rect.width * i * Vector2.right;
            position.z = 0.0f;
            go.GetComponent<RectTransform>().anchoredPosition3D = position;
            go.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        float temp = 193.0f;

        GameObject leftEnd = new GameObject("LeftTapeEnd");
        leftEnd.transform.SetParent(firstTransform.parent, false);
        Image leftEndImage = leftEnd.AddComponent<Image>();
        leftEndImage.sprite = _leftEndSprite;
        leftEndImage.SetNativeSize();
        RectTransform leftEndTransform = leftEnd.GetComponent<RectTransform>();
        if(leftEndTransform == null)
        {
            leftEndTransform = leftEnd.AddComponent<RectTransform>();
        }
        leftEndTransform.pivot = firstTransform.pivot;
        leftEndTransform.anchorMax = firstTransform.anchorMax;
        leftEndTransform.anchorMin = firstTransform.anchorMin;
        Vector3 leftEndPosition = firstTransform.anchoredPosition - Vector2.right * temp;
        leftEndPosition.z = 0.0f;
        leftEndTransform.anchoredPosition3D = leftEndPosition;
        leftEndTransform.localScale = Vector3.one;
        leftEndTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 333.5f);
        leftEndTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 193.0f);

        GameObject rightEnd = new GameObject("RightTapeEnd");
        rightEnd.transform.SetParent(firstTransform.parent, false);
        Image rightEndImage = rightEnd.AddComponent<Image>();
        rightEndImage.sprite = _rightEndSprite;
        rightEndImage.SetNativeSize();
        RectTransform rightEndTransform = rightEnd.GetComponent<RectTransform>();
        if (rightEndTransform == null)
        {
            rightEndTransform = rightEnd.AddComponent<RectTransform>();
        }
        rightEndTransform.pivot = firstTransform.pivot;
        rightEndTransform.anchorMax = firstTransform.anchorMax;
        rightEndTransform.anchorMin = firstTransform.anchorMin;
        Vector3 rightEndPosition = firstTransform.anchoredPosition + Vector2.right * (rooms.Length * firstTransform.rect.width + temp);
        rightEndPosition.z = 0.0f;
        rightEndTransform.anchoredPosition3D = rightEndPosition;
        rightEndTransform.localScale = Vector3.one;
        rightEndTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 333.5f);
        rightEndTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 193.0f);
        rightEndTransform.rotation = Quaternion.Euler(0, 0, 180.0f);

        if(firstTransform.parent != null)
        {
            firstTransform.parent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 193.0f * 3.0f + rooms.Length * firstTransform.rect.width);
        }
    }

    public override void OnDecoratorStagetBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnVisitStageBegin()
    {
        gameObject.SetActive(false);
    }

    public override void OnEscapePressed()
    {
        if(_mainMenuParent.activeInHierarchy)
        {
            ExitClick();
        }
        else
        {
            BackToMenuClick();
        }
    }

    #endregion

    #region Unity methods

    public override void OnEnable()
    {
        base.OnEnable();

        _soundImage.sprite = GameManager.Instance.SoundOn ? _soundOnSprite : _soundOffSprite;
        _mainMenuParent.SetActive(true);
        _shopMenuParent.SetActive(false);
        _exitMenuParent.SetActive(false);
        _howToPlayMenuParent.SetActive(false);
        _aboutMenuParent.SetActive(false);
    }

    void Update()
    {
        _moneyText.text = GameManager.Instance.TotalMoney.ToString("0");
    }

    #endregion

    #region Menu methods

    public void PlayClick()
    {
        GameManager.Instance.CurrentState = GameState.VisitStage;
    }

    public void ExitClick()
    {
        _mainMenuParent.SetActive(false);
        _exitMenuParent.SetActive(true);
    }

    public void ShopClick()
    {
        _mainMenuParent.SetActive(false);
        _shopMenuParent.SetActive(true);
    }

    public void AboutClick()
    {
        _mainMenuParent.SetActive(false);
        _aboutMenuParent.SetActive(true);
    }

    public void HowToPlayClick()
    {
        _mainMenuParent.SetActive(false);
        _howToPlayMenuParent.SetActive(true);
    }

    public void BackToMenuClick()
    {
        _mainMenuParent.SetActive(true);
        _exitMenuParent.SetActive(false);
        _shopMenuParent.SetActive(false);
        _howToPlayMenuParent.SetActive(false);
        _aboutMenuParent.SetActive(false);
    }

    public void SoundOnOffClick()
    {
        GameManager.Instance.SoundOn = !GameManager.Instance.SoundOn;
        _soundImage.sprite = GameManager.Instance.SoundOn ? _soundOnSprite : _soundOffSprite;

    }

    public void ExitYesClick()
    {
        Application.Quit();
    }

    public void ExitNoClick()
    {
        BackToMenuClick();
    }

    #endregion
}
