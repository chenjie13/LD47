
using System;
using DG.Tweening;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    protected Vector3Int _cell;
    public Vector3Int Cell => _cell;

    public bool CanPush = true;
    public bool IsAutoRegister;

    protected virtual void Awake()
    {
        if (IsAutoRegister) Register();
    }

    public virtual void Register()
    {
        MapMgr.Inst.RegisterItem(this);
        _cell = MapMgr.Inst.PosToCell(transform.position);
        transform.position = MapMgr.Inst.CellToPos(Cell);
    }

    public virtual Tween Move(int x, int y)
    {
        _cell.x += x;
        _cell.y += y;
        var pos = MapMgr.Inst.CellToPos(_cell.x, _cell.y);
        return transform.DOMove(pos, 0.2f);
    }
}
