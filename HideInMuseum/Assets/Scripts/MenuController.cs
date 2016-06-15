using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    private Text _soundText;
    [SerializeField]
    private Color _soundOnColor;
    [SerializeField]
    private Color _soundOffColor;

    #endregion

    #region Object Base methods

    public override void OnMenuBegin()
    {
        ExclamationMark.Instance.Reset();
        gameObject.SetActive(true);
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

        _soundText.color = GameManager.Instance.SoundOn ? _soundOnColor : _soundOffColor;
        _soundText.text = GameManager.Instance.SoundOn ? "ON" : "OFF";
        _mainMenuParent.SetActive(true);
        _shopMenuParent.SetActive(false);
        _exitMenuParent.SetActive(false);
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
        _soundText.color = GameManager.Instance.SoundOn ? _soundOnColor : _soundOffColor;
        _soundText.text = GameManager.Instance.SoundOn ? "ON" : "OFF";
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
