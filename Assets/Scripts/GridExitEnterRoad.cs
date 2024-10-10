using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExitEnterRoad : GridRoad
{
    public GridMainRoad MainRoad { get; set; }
    public GridExitStopRoad ExitStopRoad;
    public override void OnStart()
    {
        base.OnStart();
        MainRoad = levelController.FindNearestMainRoad(GetSpawnPoint());
        MainRoad.ExitEnterRoad = this;
        ExitStopRoad.GridExitEnterRoad = this;
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
    }
}
