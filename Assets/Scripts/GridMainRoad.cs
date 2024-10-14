using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMainRoad : GridRoad, INeighborable<GridMainRoad>
{
    private GridMainRoad upMainRoad;
    private GridMainRoad botMainRoad;
    private GridMainRoad leftMainRoad;
    private GridMainRoad rightMainRoad;

    [SerializeField] bool isExitMainRoad = false;

    public List<GridExitEnterRoad> ExitEnterRoads { get; set; } = new List<GridExitEnterRoad>();

    public override void OnStart()
    {
        base.OnStart();
        spawnPoint = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        //transform.localPosition = new Vector3(spawnPoint.x, -0.3f, spawnPoint.y);
    }
    public GridMainRoad Up
    {
        get => upMainRoad;
        set => upMainRoad = value;
    }

    public GridMainRoad Bot
    {
        get => botMainRoad;
        set => botMainRoad = value;
    }

    public GridMainRoad Left
    {
        get => leftMainRoad;
        set => leftMainRoad = value;
    }

    public GridMainRoad Right
    {
        get => rightMainRoad;
        set => rightMainRoad = value;
    }

    public void SetIsExitRoad(bool isExitRoad)
    {
        this.isExitMainRoad = isExitRoad;
    }

    public bool GetIsExitRoad() { return isExitMainRoad; }
}

public interface INeighborable<T>
{
    T Up { get; set; }
    T Bot { get; set; }
    T Left { get; set; }
    T Right { get; set; }
}
