
using CGDK.Core;
using UnityEngine;

public class MapDoor : MonoBehaviour
{
    public int Id;
    public Sprite OpenSprite;
    
    public  Vector3Int Cell { get; private set; }
    public bool IsOpen { get; private set; }

    private Sprite _closeSprite;
    private SpriteRenderer _render;
    
    public void Init()
    {
        _render = GetComponent<SpriteRenderer>();
        _closeSprite = _render.sprite;
        Cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(Cell);
        
        EventBus.Subscribe<SwitchEvent>(OnSwitchEvent);
    }
    
    private void OnSwitchEvent(SwitchEvent e)
    {
        if (e.Id != Id) return;
        IsOpen = e.IsOpen;
        _render.sprite = IsOpen ? OpenSprite : _closeSprite;
    }
    
    private void OnDestroy()
    {        
        EventBus.Unsubscribe<SwitchEvent>(OnSwitchEvent);
    }
}
