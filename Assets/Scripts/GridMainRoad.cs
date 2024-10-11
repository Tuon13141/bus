using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMainRoad : GridRoad, INeighborable<GridMainRoad>
{
    private GridMainRoad upMainRoad;
    private GridMainRoad botMainRoad;
    private GridMainRoad leftMainRoad;
    private GridMainRoad rightMainRoad;

    [SerializeField] bool isExitMap = false;

    public List<GridExitEnterRoad> ExitEnterRoads { get; set; } = new List<GridExitEnterRoad>();

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
        this.isExitMap = isExitRoad;
    }
}

public interface INeighborable<T>
{
    T Up { get; set; }
    T Bot { get; set; }
    T Left { get; set; }
    T Right { get; set; }
}
