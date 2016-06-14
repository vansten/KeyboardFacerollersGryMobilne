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

        _soundText.color = GameManager.Instance.SoundOn ? _soundOnColor : _soundOffColor;
        _soundText.text = GameManager.Instance.SoundOn ? "ON" : "OFF";
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
