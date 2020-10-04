
using System;
using CGDK.Core;
using UnityEngine;

public class RobotSpawner : MonoBehaviour
{
    public Robot Prefab;
    public CommandType FirstCommand;
    public int CommandCount;
    public int DisableHolderCount = 1;
    public int TimelineId;
    public float DelayTime;

    public  Vector3Int Cell { get; private set; }

    public void Init()
    {
        var timeline = GameMgr.Inst.GetTimelineById(TimelineId);
        if (timeline == null) Debug.LogError("Cant found timeline " + TimelineId);
        var firstHolder = timeline.GetCommandHolder(0);
        for (int i = 0; i < GameMgr.Inst.Configs.Count; i++)
        {
            var conf = GameMgr.Inst.Configs[i];
            if (conf.CommandType == FirstCommand)
            {
                firstHolder.SetCommandConf(conf);
                break;
            }
        }

        for (int i = 0; i < DisableHolderCount; i++)
        {
            var holder = timeline.GetCommandHolder(i);
            if (holder != null) holder.Editable = false;
        }
        
        timeline.SetCommandCount(CommandCount);

        Cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(Cell);
        
        var canvas = GetComponentInChildren<Canvas>(true);
        canvas.worldCamera = Camera.main;
        
        EventBus.Subscribe<CommandEvent>(OnCommandEvent);
    }

    public void SpawnRobot()
    {
        if (MapMgr.Inst.HasMapItem(Cell)) 
            return;
        
        var robot = Instantiate(Prefab, transform.position, Quaternion.identity);
        robot.Init(TimelineId, DelayTime);
    }

    private void OnCommandEvent(CommandEvent e)
    {
        if (e.PhaseType != CommandPhaseType.Start) return;
        if (e.TimelineId != TimelineId) return;
        SpawnRobot();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<CommandEvent>(OnCommandEvent);
    }
}
