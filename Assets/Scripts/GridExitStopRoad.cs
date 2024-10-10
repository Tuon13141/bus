using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExitStopRoad : GridRoad
{
    public GridExitEnterRoad GridExitEnterRoad { get; set; }

    public override void OnStart()
    {
        base.OnStart();
        transform.position = new Vector3(spawnPoint.x, 0, spawnPoint.y);
    }
}
