using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CommandType
{
    None, Left, Right, Up, Down, Wait,
}

[Serializable]
public class CommandPickerConfig
{
    public CommandType CommandType;
    public Sprite CommandSprite;
    public KeyCode ShortcutKey;
}

public class CommandPickerBar : MonoBehaviour
{
    public GameObject Prefab;

    private void Start()
    {
        foreach (var conf in GameMgr.Inst.Configs)
        {
            var picker = Instantiate(Prefab, transform);
            var image = picker.transform.Find("CommandImg").GetComponent<Image>();
            image.sprite = conf.CommandSprite;
            var text = picker.GetComponentInChildren<Text>();
            text.text = conf.ShortcutKey.ToString();
            if (conf.CommandType == CommandType.None)
            {
                var group = picker.GetComponent<CanvasGroup>();
                group.alpha = 0;
            }
        }
    }
}
