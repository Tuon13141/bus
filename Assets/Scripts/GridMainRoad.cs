using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMainRoad : GridRoad
{
    public GridMainRoad UpMainRoad { get; set; }
    public GridMainRoad BotMainRoad { get; set; }
    public GridMainRoad LeftMainRoad { get; set; }
    public GridMainRoad RightMainRoad { get; set; }

    [SerializeField] bool isExitRoad = false;

    public GridExitEnterRoad ExitEnterRoad { get; set; }

    public void SetIsExitRoad(bool isExitRoad)
    {
        this.isExitRoad = isExitRoad;
    }
}
