
using System;
using System.Collections;
using System.Collections.Generic;
using CGDK.Core;
using Doozy.Engine.Soundy;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum GameEvent
{
    LoadLevelEnd, RunTimeline, StopTimeline, PauseTimeline, Restart
}

public class GameMgr : MonoSingleton<GameMgr>
{
    public bool IsTestLevel;
    public List<CommandPickerConfig> Configs;
    public GameObject GlobalMask;
    public UnityEvent OnPlay;
    public UnityEvent OnPause;
    public UnityEvent OnStop;
    public List<string> LevelTitles;
    public Text TitleText;
    public GameObject GameOverUI;

    [SerializeField]
    private float _runStepTime = 0.5f;
    
    public float RunStepTime
    {
        get { return _runStepTime; }
    }
    
    public bool IsRunning { get; private set; }
    public bool IsPause { get; private set; }

    private Dictionary<int, CommandTimeline> _timelines = new Dictionary<int, CommandTimeline>();
    private int _pointCount;
    private int _finishPointCount;

    private int _nextLevelId = 0;
    private GameObject _lastLevelObj;
    private Canvas _canvas;
    private Animator _titleAnim;

    private void Start()
    {
        _titleAnim = TitleText.GetComponent<Animator>();
        foreach (var timeline in FindObjectsOfType<CommandTimeline>())
        {
            _timelines.Add(timeline.TimelineId, timeline);
        }
        
        LoadNextLevel();
        // SoundyManager.Play("Example Music", "Little Ludvig");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SoundyManager.Play("Example Clicks", "Bubble Pop");
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(CoLoadLevel(_nextLevelId));
        _nextLevelId++;
    }

    private IEnumerator CoLoadLevel(int id)
    {
        GlobalMask.SetActive(true);
        Pasue();
        if (_lastLevelObj != null)
        {
            yield return new WaitForSeconds(1.2f);
            Stop();
            Destroy(_lastLevelObj);
            _lastLevelObj = null;
            yield return null;
        }

        if (id >= LevelTitles.Count)
        {
            GameOverUI.SetActive(true);
            Debug.Log("Clear All Levels " + id);
            yield break;
        }
        
        TitleText.text = $"Level {id + 1}: {LevelTitles[id]}";
        _titleAnim.Play("LevelTitleShow", 0);
        var levelPrefix = IsTestLevel ? "Levels/Test/Level_" : "Levels/Level_";
        var prefab = Resources.Load(levelPrefix + id);
        _lastLevelObj = Instantiate(prefab) as GameObject;

        _canvas = null;
        var canvasTrans = _lastLevelObj.transform.Find("Canvas");
        if (canvasTrans != null)
        {
            _canvas = canvasTrans.GetComponent<Canvas>();
        }
        if (_canvas != null)
        {
            _canvas.worldCamera = Camera.main;
        }
        
        yield return null;

        foreach (var timeline in _timelines.Values)
        {
            timeline.Clear();
            timeline.SetCommandCount(0);
        }

        EventBus.Trigger(GameEvent.LoadLevelEnd);
        
        var points = FindObjectsOfType<RobotEndPoint>();
        foreach (var point in points)
        {
            point.Init();   
        }

        _finishPointCount = 0;
        _pointCount = points.Length;
        GlobalMask.SetActive(false);
    }

    public void RestartGame()
    {
        GameOverUI.SetActive(false);
        _nextLevelId = 0;
        EventBus.Trigger(GameEvent.Restart);
        LoadNextLevel();
    }

    public void Run()
    {
        if (_canvas != null)
        {
            _canvas.gameObject.SetActive(false);
        }
        
        EventBus.Trigger(GameEvent.RunTimeline);
        IsPause = false;
        IsRunning = true;
        foreach (var timeline in _timelines.Values)
        {
            timeline.Run();
        }
        OnPlay.Invoke();
    }

    public void Stop()
    {
        EventBus.Trigger(GameEvent.StopTimeline);
        if (!IsRunning) return;
        IsRunning = false;
        _finishPointCount = 0;
        foreach (var timeline in _timelines.Values)
        {
            timeline.Stop();
        }
        OnStop.Invoke();
    }

    public void Pasue()
    {
        if (!IsRunning) return;
        EventBus.Trigger(GameEvent.PauseTimeline);
        if (IsPause) return;
        IsPause = true;
        foreach (var timeline in _timelines.Values)
        {
            timeline.Pasue();
        }
        OnPause.Invoke();
    }

    public bool TryFinishLevel()
    {
        _finishPointCount++;
        if (_finishPointCount >= _pointCount)
        {
            Debug.Log("FinishLevel");
            LoadNextLevel();
            return true;
        }

        return false;
    }

    public CommandTimeline GetTimelineById(int id)
    {
        if (_timelines.ContainsKey(id)) return _timelines[id];
        return null;
    }
}
