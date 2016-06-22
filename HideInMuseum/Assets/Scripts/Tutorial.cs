using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TutorialStage
{
    TS_Exclamation,
    TS_GroupIcons,
    TS_GroupObjectives,
    TS_GroupSad,
    TS_Satisfaction,
    TS_Fault,
    TS_FirstGroupMovement,
    TS_Gyroscope
}

[Serializable]
public class TutorialObject
{
    public TutorialStage Stage;
    public GameObject TextObject;
    public GameObject Arrow;
    [HideInInspector]
    public bool Shown;

    public void SetActive(bool active)
    {
        TextObject.SetActive(active);
        Arrow.SetActive(active);
    }
}

public class Tutorial : Singleton<Tutorial>
{
    public List<TutorialObject> TutorialObjects;
    [SerializeField]
    private GameObject _popup;

    private Dictionary<TutorialStage, TutorialObject> _stageToObjectDictionary = new Dictionary<TutorialStage, TutorialObject>();
    private TutorialObject _shownObject;
    private bool _tutorialShown;

    public void OKClick()
    {
        _tutorialShown = false;
        _shownObject.SetActive(false);
        _popup.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void SkipClick()
    {
        _tutorialShown = false;
        _popup.SetActive(false);
        _shownObject.SetActive(false);
        foreach(TutorialObject to in TutorialObjects)
        {
            PlayerPrefs.SetInt(to.Stage.ToString() + "_shown", 1);
        }
        PlayerPrefs.Save();
        Time.timeScale = 1.0f;
    }

    public void Init()
    {
        foreach (TutorialObject to in TutorialObjects)
        {
            if (!_stageToObjectDictionary.ContainsKey(to.Stage))
            {
                _stageToObjectDictionary.Add(to.Stage, to);
            }
            to.Shown = false;
        }
        _tutorialShown = false;
        _shownObject = null;
    }

    public bool IsShown()
    {
        return _tutorialShown;
    }

    public void ShowTutorial(TutorialStage stage)
    {
        if(GameManager.Instance.CurrentState != GameState.VisitStage)
        {
            return;
        }

        bool show = PlayerPrefs.GetInt(stage.ToString() + "_shown", 0) == 0;
#if DEVELOPMENT_BUILD
        show = true;
#endif
        TutorialObject to = null;
        if (_stageToObjectDictionary.ContainsKey(stage))
        {
            to = _stageToObjectDictionary[stage];
        }

        if (!show)
        {
            to.Shown = true;
            return;
        }

        if(to != null && !to.Shown)
        {
            PlayerPrefs.SetInt(stage.ToString() + "_shown", 1);
            PlayerPrefs.Save();
            _tutorialShown = true;
            _popup.SetActive(true);
            to.SetActive(true);
            to.Shown = true;
            Time.timeScale = 0.0f;
        }
        _shownObject = to;
    }
}
