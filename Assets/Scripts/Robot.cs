
using System;
using CGDK.Core;
using DG.Tweening;
using UnityEngine;

public class Robot : MapItem
{
    public Sprite EndSprite;
    public int TimelineId;

    private CommandType _lastCmd;
    private SpriteRenderer _render;
    private ItemPurpose _purpose;
    private float _delay;

    public void Init(int timelineId, float delay = 0)
    {
        _delay = delay;
        _purpose = GetComponentInChildren<ItemPurpose>();
        _render = GetComponent<SpriteRenderer>();
        TimelineId = timelineId;
        EventBus.Subscribe<CommandEvent>(OnCommandEvent);
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }

    public override Tween Move(int x, int y)
    {
        return base.Move(x, y).OnComplete(() =>
        {
            var e = new CommandEvent();
            e.CommandType = _lastCmd;
            e.TimelineId = TimelineId;
            e.Item = this;
            e.PhaseType = CommandPhaseType.AfterRun;
            EventBus.Trigger(e);
        });
    }

    private void OnDestroy()
    {
        MapMgr.Inst.UnregisterItem(this);
        EventBus.Unsubscribe<CommandEvent>(OnCommandEvent);
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
    }

    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.StopTimeline)
        {
            Destroy(gameObject);
        }
    }

    private void OnCommandEvent(CommandEvent e)
    {
        if (e.TimelineId != TimelineId) return;

        TimerUtil.ScaledTimer.WaitFor(_delay).Then(() =>
        {
            if (e.PhaseType == CommandPhaseType.Run)
            {
                if (e.CommandType == CommandType.None)
                {
                    DoCommand(_lastCmd);
                }
                else
                {
                    DoCommand(e.CommandType);
                    _lastCmd = e.CommandType;
                }
            }

            if (e.PhaseType == CommandPhaseType.End)
            {
                _render.sprite = EndSprite;
                EventBus.Unsubscribe<CommandEvent>(OnCommandEvent);
            }
        });
    }

    public void DoCommand(CommandType cmd)
    {
        switch (cmd)
        {
            case CommandType.Up:
                MapMgr.Inst.HandleItemMove(this, 0, 1);
                break;
            case CommandType.Down:
                MapMgr.Inst.HandleItemMove(this, 0, -1);
                break;
            case CommandType.Left:
                MapMgr.Inst.HandleItemMove(this, -1, 0);
                break;
            case CommandType.Right:
                MapMgr.Inst.HandleItemMove(this, 1, 0);
                break;
            case CommandType.Wait:
                Wait();
                break;
        }
    }

    public void Wait()
    {
        // if (_purpose != null) _purpose.ShowWaitPurpose();
    }
}
