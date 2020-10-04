
using CGDK.Core;
using UnityEngine;

public class MapBox : MapItem
{
    private Vector3Int _initCell;

    public void Init()
    {
        _cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(_cell);
        _initCell = _cell;
        Register();
        
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }

    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.StopTimeline)
        {
            _cell = _initCell;
            transform.position = MapMgr.Inst.CellToPos(_cell);
        }
    }

    private void OnDestroy()
    {
        MapMgr.Inst.UnregisterItem(this);
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
    }
}
