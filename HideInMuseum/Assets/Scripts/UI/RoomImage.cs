using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomImage : MonoBehaviour
{
    public Image Image;
    public Button BuyButton;
    public Image Icon;
    public Text Name;
    public Text MoneyRequired;
    public Image MoneyImage;
    public AudioClip PurchaseSound;

    private RoomInfo _roomInfo;

    public void SetRoom(RoomInfo ri)
    {
        _roomInfo = ri;
        Image.sprite = ri.ShopImage;
        Icon.sprite = ri.Icon;
        Name.text = ri.Name;
        BuyButton.interactable = !ri.Room.Unlocked;
        MoneyRequired.gameObject.SetActive(!ri.Room.Unlocked);
        MoneyImage.gameObject.SetActive(!ri.Room.Unlocked);
        MoneyRequired.text = ri.MoneyToUnlock.ToString();
    }

    public void BuyClick()
    {
        GameManager.Instance.TotalMoney += _roomInfo.MoneyToUnlock;
        if(GameManager.Instance.TotalMoney >= _roomInfo.MoneyToUnlock)
        {
            GameManager.Instance.TotalMoney -= _roomInfo.MoneyToUnlock;
            _roomInfo.Room.Unlocked = true;
            BuyButton.interactable = false;
            MoneyRequired.gameObject.SetActive(false);
            MoneyImage.gameObject.SetActive(false);
            GameManager.Instance.SaveData();
            AudioSource.PlayClipAtPoint(PurchaseSound, Camera.main.transform.position, 0.75f);
        }
    }
}
