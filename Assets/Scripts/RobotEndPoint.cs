
using System;
using CGDK.Core;
using UnityEngine;
using UnityEngine.UI;

public class RobotEndPoint : MonoBehaviour
{
    public Color FinishColor;
    public int FinishRobotCount;
    public int TimelineId;
    public bool AcceptAllRobot;
    
    private Text _desText;
    private SpriteRenderer _render;
    private Color _rowColor;

    public  Vector3Int Cell { get; private set; }
    public int CurCount { get; private set; }
    public bool IsFinish { get; private set; }
    
    public void Init()
    {
        Cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(Cell);

        _render = GetComponent<SpriteRenderer>();
        _rowColor = _render.color;
        _desText = GetComponentInChildren<Text>(true);
        var canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        _desText.text = $"{CurCount}/{FinishRobotCount}";
        _desText.gameObject.SetActive(true);

        EventBus.Subscribe<CommandEvent>(OnCommandEvent);
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }

    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.StopTimeline)
        {
            CurCount = 0;
            _desText.text = $"{CurCount}/{FinishRobotCount}";
            // _desText.color = Color.white;
            _render.color = _rowColor;
            _desText.gameObject.SetActive(true);
        }
    }

    private void OnCommandEvent(CommandEvent e)
    {
        if (CurCount == FinishRobotCount) return;
        
        if (e.PhaseType != CommandPhaseType.AfterRun) return;
        if (!AcceptAllRobot && e.TimelineId != TimelineId) return;

        var item = MapMgr.Inst.GetMapItem(Cell);
        if (item == null) return;
        var robot = item as Robot;
        if (robot == null || e.Item != robot) return;

        Destroy(robot.gameObject, 0.1f);
        CurCount++;
        _desText.text = $"{Mathf.Min(CurCount, FinishRobotCount)}/{FinishRobotCount}";
        
        if (CurCount == FinishRobotCount)
        {
            _desText.gameObject.SetActive(false);
            // _desText.color = FinishColor;
            _render.color = FinishColor;
            IsFinish = true;
            GameMgr.Inst.TryFinishLevel();
        }
    }

    private void OnDestroy()
    {        
        EventBus.Unsubscribe<CommandEvent>(OnCommandEvent);
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
    }
}
