using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : ObjectBase
{
    #region Private variables

    [SerializeField]
    private GameObject _mainMenuParent;
    [SerializeField]
    private GameObject _highscoresMenuParent;
    [SerializeField]
    private GameObject _exitMenuParent;
    [SerializeField]
    private Text _soundText;
    [SerializeField]
    private Color _soundOnColor;
    [SerializeField]
    private Color _soundOffColor;

    private bool _soundOn;

    #endregion

    #region Object Base methods

    public override void OnMenuBegin()
    {
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

        _soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        _soundText.color = _soundOn ? _soundOnColor : _soundOffColor;
        _soundText.text = _soundOn ? "ON" : "OFF";
        _mainMenuParent.SetActive(true);
        _highscoresMenuParent.SetActive(false);
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

    public void HighscoresClick()
    {
        _mainMenuParent.SetActive(false);
        _highscoresMenuParent.SetActive(true);
    }

    public void BackToMenuClick()
    {
        _mainMenuParent.SetActive(true);
        _exitMenuParent.SetActive(false);
        _highscoresMenuParent.SetActive(false);
    }

    public void SoundOnOffClick()
    {
        _soundOn = !_soundOn;
        _soundText.color = _soundOn ? _soundOnColor : _soundOffColor;
        _soundText.text = _soundOn ? "ON" : "OFF";
        PlayerPrefs.SetInt("SoundOn", _soundOn ? 1 : 0);
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
