using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomImage : MonoBehaviour
{
    public Image Image;
    public GameObject BuyButton;
    public Image Icon;
    public Text Name;

    private RoomInfo _roomInfo;

    public void SetRoom(RoomInfo ri)
    {
        _roomInfo = ri;
        Image.sprite = ri.ShopImage;
        Icon.sprite = ri.Icon;
        Name.text = ri.Name;
        BuyButton.SetActive(!ri.Room.Unlocked);
        BuyButton.GetComponentInChildren<Text>().text = ri.MoneyToUnlock.ToString();
    }

    public void BuyClick()
    {
        if(GameManager.Instance.TotalMoney >= _roomInfo.MoneyToUnlock)
        {
            GameManager.Instance.TotalMoney -= _roomInfo.MoneyToUnlock;
            _roomInfo.Room.Unlocked = true;
            BuyButton.SetActive(false);
        }
    }
}
