using UnityEngine;
using System.Collections;

public class MenuController : ObjectBase
{
    #region Private variables

    [SerializeField]
    private GameObject _mainMenuParent;
    [SerializeField]
    private GameObject _settingsMenuParent;
    [SerializeField]
    private GameObject _aboutMenuParent;

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

    #endregion

    #region Unity methods

    void OnEnable()
    {
        _mainMenuParent.SetActive(true);
        _settingsMenuParent.SetActive(false);
        _aboutMenuParent.SetActive(false);
    }

    #endregion

    #region Menu methods

    public void PlayClick()
    {
        GameManager.Instance.CurrentState = GameState.VisitStage;
    }

    public void AboutClick()
    {
        _mainMenuParent.SetActive(false);
        _aboutMenuParent.SetActive(true);
    }

    public void SettingsClick()
    {
        _mainMenuParent.SetActive(false);
        _settingsMenuParent.SetActive(true);
    }

    public void BackToMenuClick()
    {
        _mainMenuParent.SetActive(true);
        _aboutMenuParent.SetActive(false);
        _settingsMenuParent.SetActive(false);
    }

    #endregion
}
