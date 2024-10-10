using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBorderRoad : GridRoad
{
    public GridBorderRoad UpBorderRoad { get; set; }
    public GridBorderRoad BotBorderRoad { get; set; }
    public GridBorderRoad LeftBorderRoad { get; set; }
    public GridBorderRoad RightBorderRoad { get; set; }

    public List<GridMainRoad> MainRoads { get; set; } = new List<GridMainRoad>();
}
