using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBorderRoad : GridRoad, INeighborable<GridBorderRoad>
{
    private GridBorderRoad upBorderRoad;
    private GridBorderRoad botBorderRoad;
    private GridBorderRoad leftBorderRoad;
    private GridBorderRoad rightBorderRoad;

    public GridMainRoad MainRoad { get; set; }
    public override void OnStart()
    {
        base.OnStart();
    }

    public GridBorderRoad Up
    {
        get => upBorderRoad;
        set => upBorderRoad = value;
    }

    public GridBorderRoad Bot
    {
        get => botBorderRoad;
        set => botBorderRoad = value;
    }

    public GridBorderRoad Left
    {
        get => leftBorderRoad;
        set => leftBorderRoad = value;
    }

    public GridBorderRoad Right
    {
        get => rightBorderRoad;
        set => rightBorderRoad = value;
    }

    public override void ResetParameter()
    {
        base.ResetParameter();
    }
}
