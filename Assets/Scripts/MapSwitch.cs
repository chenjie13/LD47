
using CGDK.Core;
using UnityEngine;

public struct SwitchEvent
{
    public int Id;
    public bool IsOpen;
}

public class MapSwitch : MonoBehaviour
{
    public int Id;
    public  Vector3Int Cell { get; private set; }
    
    public void Init()
    {
        Cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(Cell);
        
        EventBus.Subscribe<CommandEvent>(OnCommandEvent);
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }
    
    private void OnCommandEvent(CommandEvent e)
    {
        if (e.PhaseType != CommandPhaseType.AfterRun) return;
        
        var item = MapMgr.Inst.GetMapItem(Cell);
        var switchEvent = new SwitchEvent();
        switchEvent.Id = Id;
        switchEvent.IsOpen = item != null;
        EventBus.Trigger(switchEvent);
    }
    
    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.StopTimeline)
        {
            var switchEvent = new SwitchEvent();
            switchEvent.Id = Id;
            switchEvent.IsOpen = false;
            EventBus.Trigger(switchEvent);
        }
    }

    private void OnDestroy()
    {        
        EventBus.Unsubscribe<CommandEvent>(OnCommandEvent);
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
    }
}
