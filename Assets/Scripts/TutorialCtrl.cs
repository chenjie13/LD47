
using System;
using CGDK.Core;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCtrl : MonoBehaviour
{
    public int ShowLevel;

    private int _levelCount;
    private Button _button;
    
    private void Awake()
    {
        gameObject.SetActive(false);
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }

    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.LoadLevelEnd)
        {
            _levelCount++;
            if (_levelCount == ShowLevel)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else if (e == GameEvent.RunTimeline)
        {
            gameObject.SetActive(false);
        }
        else if (e == GameEvent.Restart)
        {
            _levelCount = 0;
            gameObject.SetActive(false);
        }
    }
}
