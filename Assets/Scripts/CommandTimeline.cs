
using System;
using System.Collections.Generic;
using CGDK.Core;
using UnityEngine;

public enum CommandPhaseType { Run, AfterRun, Start, End, }

public struct CommandEvent
{
    public CommandPhaseType PhaseType;
    public CommandType CommandType;
    public int TimelineId;
    public MapItem Item;
}

public class CommandTimeline : MonoBehaviour
{
    public float StartDelay = 0.15f;
    public int TimelineId = 1;

    public bool IsRunning { get; private set; }
    public bool IsPause { get; private set; }
    
    public int ActiveHolderCount { get; private set; }

    private bool _isDelay = true;
    private float _runTimer;
    private int _nextCmdIndex;
    private CommandHolder _runningHolder;

    private List<CommandHolder> _holders = new List<CommandHolder>();

    private void Awake()
    {
        _holders.AddRange(GetComponentsInChildren<CommandHolder>());
    }

    private void Update()
    {
        if (!IsRunning || IsPause || ActiveHolderCount == 0) return;

        _runTimer += Time.deltaTime;
        
        if (_isDelay)
        {
            if (_runTimer > StartDelay)
            {
                _runTimer = 0;
                _isDelay = false;
            }
            return;
        }
        
        if (_runTimer > GameMgr.Inst.RunStepTime)
        {
            _runTimer = 0;
            DoNextCmd();
        }
    }

    private void DoNextCmd()
    {
        if (_runningHolder != null) _runningHolder.PreviewRunning(false);
        _runningHolder = _holders[_nextCmdIndex];
        _runningHolder.PreviewRunning(true);

        var e = new CommandEvent();
        e.CommandType = _runningHolder.CommandType;
        e.TimelineId = TimelineId;

        if (_nextCmdIndex == 0)
        {
            e.PhaseType = CommandPhaseType.Start;
            EventBus.Trigger(e);
        }
        
        e.PhaseType = CommandPhaseType.Run;
        EventBus.Trigger(e);
        
        if (++_nextCmdIndex >= ActiveHolderCount)
        {
            _nextCmdIndex = 0;
            e.PhaseType = CommandPhaseType.End;
            EventBus.Trigger(e);
        }
    }

    public void Clear()
    {
        foreach (var holder in _holders)
        {
            holder.Remove();
            holder.Editable = true;
            holder.gameObject.SetActive(false);
        }
    }

    public void Run()
    {
        IsPause = false;
        IsRunning = true;
        foreach (var holder in _holders)
        {
            holder.Select(false);
            holder.EnableBtnNav(false);
        }
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        
        _nextCmdIndex = 0;
        _runTimer = 0;
        _isDelay = true;
        
        foreach (var holder in _holders)
        {
            holder.PreviewRunning(false);
            holder.EnableBtnNav(true);
        }
    }

    public void Pasue()
    {
        if (IsPause) return;
        IsPause = true;
    }

    public CommandHolder GetCommandHolder(int index)
    {
        return _holders[index];
    }

    public void SetCommandCount(int count)
    {
        ActiveHolderCount = count;
        for (int i = 0; i < _holders.Count; i++)
        {
            if (i < count)
            {
                var holder = _holders[i];
                holder.gameObject.SetActive(true);
            }
        }
    }
}
