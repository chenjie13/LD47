
using System;
using Doozy.Engine.Soundy;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class CommandHolder : MonoBehaviour
{
    public Image RunningImg;
    public Image CommandImg;
    public GameObject EditableObj;
    public GameObject NotEditableObj;
    public CommandType CommandType;

    private bool _editable = true;
    public bool Editable
    {
        get { return _editable; }
        set
        {
            if (_editable == value) return;
            _editable = value;
            _uiBtn.DeselectButtonAfterClick = !_editable;
            EditableObj.SetActive(_editable);
            NotEditableObj.SetActive(!_editable);
            var nav = _uiBtn.Button.navigation;
            if (_editable)
            {
                _uiBtn.OnClick.LoadPreset("Scale", "ClickInFast");
                nav.mode = Navigation.Mode.Explicit;
            }
            else
            {
                _uiBtn.OnClick.LoadPreset("Slide", "VibrateFromCenter");
                nav.mode = Navigation.Mode.None;
            }
            _uiBtn.Button.navigation = nav;
        }
    }
    
    private UIButton _uiBtn;

    private void Awake()
    {
        _uiBtn = GetComponent<UIButton>();
    }

    private void Update()
    {
        if (!_editable || GameMgr.Inst.IsRunning || !_uiBtn.IsSelected) return;

        foreach (var conf in GameMgr.Inst.Configs)
        {
            if (Input.GetKeyDown(conf.ShortcutKey))
            {
                SoundyManager.Play("Example Clicks", "Button Tap");
                SetCommandConf(conf);
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            SoundyManager.Play("Example Clicks", "Button Tap");
            Remove();
        }
    }

    public void SetCommandConf(CommandPickerConfig conf)
    {
        if (conf.CommandType == CommandType.None) return;

        CommandImg.sprite = conf.CommandSprite;
        CommandImg.gameObject.SetActive(true);
        CommandType = conf.CommandType;
    }

    public void Remove()
    {
        CommandImg.sprite = null;
        CommandImg.gameObject.SetActive(false);
        CommandType = CommandType.None;
    }

    public void PreviewRunning(bool val)
    {
        RunningImg.gameObject.SetActive(val);
    }
    
    public void Select(bool val)
    {
        if (val) _uiBtn.SelectButton();
        else _uiBtn.DeselectButton();
    }

    public void EnableBtnNav(bool val)
    {
        if (!_editable) return;
        var nav = _uiBtn.Button.navigation;
        nav.mode = Navigation.Mode.None;
        if (val) nav.mode = Navigation.Mode.Explicit;
        _uiBtn.Button.navigation = nav;
    }
}
