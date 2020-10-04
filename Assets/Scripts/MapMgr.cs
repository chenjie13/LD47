using System.Collections.Generic;
using CGDK.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMgr : MonoSingleton<MapMgr>
{
    private Tilemap _tilemap;
    private List<MapItem> _items = new List<MapItem>();
    private List<MapDoor> _doors = new List<MapDoor>();
    
    private void Awake()
    {
        EventBus.Subscribe<GameEvent>(OnGameEvent);
    }

    private void OnGameEvent(GameEvent e)
    {
        if (e == GameEvent.LoadLevelEnd)
        {
            _tilemap = FindObjectOfType<Tilemap>();
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }
            _items.Clear();
            foreach (var spawner in FindObjectsOfType<RobotSpawner>())
            {
                spawner.Init();
            }
            foreach (var box in FindObjectsOfType<MapBox>())
            {
                box.Init();
            }
            foreach (var s in FindObjectsOfType<MapSwitch>())
            {
                s.Init();
            }
            _doors.Clear();
            _doors.AddRange(FindObjectsOfType<MapDoor>());
            foreach (var door in _doors)
            {
                door.Init();
            }
        }
    }
    
    public bool HandleItemMove(MapItem item, int x, int y)
    {
        var cell = item.Cell;
        var purpose = item.GetComponentInChildren<ItemPurpose>();
        var items = new List<MapItem>();
        items.Add(item);
        bool canPush = false;
        while (true)
        {
            cell.x += x;
            cell.y += y;
            
            if (!_tilemap.HasTile(cell)) break;
            
            var door = GetDoor(cell);
            if (door != null && !door.IsOpen) break;
            
            if (HasMapItem(cell))
            {
                var i = GetMapItem(cell);
                if (i.CanPush)
                    items.Add(i);
                else
                    break;
            }
            else
            {
                canPush = true;
                break;
            }
        }

        if (!canPush)
        {
            if (purpose != null)
            {
                purpose.ShowCantMovePurpose(x, y);
            }
            return false;
        }
        
        foreach (var i in items)
        {
            i.Move(x, y);
        }
        if (purpose != null)
        {
            purpose.ShowMovePurpose(x, y);
        }
        return true;
    }
    
    public MapDoor GetDoor(Vector3Int cell)
    {
        foreach (var door in _doors)
        {
            if (door != null && door.Cell == cell) return door;
        }

        return null;
    }

    public bool HasMapItem(Vector3Int cell)
    {
        foreach (var item in _items)
        {
            if (item != null && item.Cell == cell) return true;
        }

        return false;
    }
    
    public MapItem GetMapItem(Vector3Int cell)
    {
        foreach (var item in _items)
        {
            if (item.Cell == cell) return item;
        }

        return null;
    }
    
    public Vector3Int PosToCell(Vector3 pos)
    {
        return _tilemap.WorldToCell(pos);
    }

    public Vector3 CellToPos(Vector3Int cell)
    {
        return _tilemap.CellToWorld(cell) + _tilemap.layoutGrid.cellSize / 2;
    }
    
    public bool HasTile(Vector3Int cell)
    {
        return _tilemap.HasTile(cell);
    }
    
    public Vector3 CellToPos(int x, int y)
    {
        return CellToPos(new Vector3Int(x, y, 0));
    }

    public void RegisterItem(MapItem item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);
        }
    }
    
    public void UnregisterItem(MapItem item)
    {
        _items.Remove(item);
    }

    protected override void OnDestroy()
    {
        EventBus.Unsubscribe<GameEvent>(OnGameEvent);
        base.OnDestroy();
    }
}
