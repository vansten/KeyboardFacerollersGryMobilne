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
    private bool _skipped;

    public void OKClick()
    {
        if(_shownObject == null)
        {
            Debug.LogError("There is no shown object but it's possible to click OK");
            return;
        }

        _shownObject.SetActive(false);
        _popup.SetActive(false);
        EndShowTutorial(TutorialStage.TS_GroupSad);
        Time.timeScale = 1.0f;
    }

    public void SkipClick()
    {
        if (_shownObject == null)
        {
            Debug.LogError("There is no shown object but it's possible to click Skip");
            return;
        }

        Tutorial.Instance.EndShowTutorial(TutorialStage.TS_GroupSad);
        _popup.SetActive(false);
        _shownObject.SetActive(false);
        _skipped = true;
        _tutorialShown = false;
        PlayerPrefs.SetInt("TutorialSkipped", 1);
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
        _skipped = PlayerPrefs.GetInt("TutorialSkipped", 0) == 1;
    }

    public bool IsPopupActive()
    {
        return _popup.activeInHierarchy;
    }

    public void ShowTutorial(TutorialStage stage)
    {
        if(GameManager.Instance.CurrentState != GameState.VisitStage || _tutorialShown || _skipped || _shownObject != null)
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

    public void EndShowTutorial(TutorialStage stage)
    {
        if(_shownObject == null || !_tutorialShown)
        {
            return;
        }

        if(_shownObject.Stage != stage)
        {
            return;
        }

        _tutorialShown = false;
        _shownObject = null;
    }

    public bool WasShown(TutorialStage stage)
    {
        if(_skipped)
        {
            return true;
        }

        return PlayerPrefs.GetInt(stage.ToString() + "_shown", 0) == 1;
    }
}
