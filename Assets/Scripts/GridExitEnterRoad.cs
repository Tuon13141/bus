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
        MainRoad = levelController.FindNearestMainRoad(new Vector2Int((int)GetTransformPosition().x, (int)GetTransformPosition().z));
        MainRoad.ExitEnterRoads.Add(this);
        ExitStopRoad.GridExitEnterRoad = this;
        transform.localPosition = new Vector3(spawnPoint.x, 0, spawnPoint.y);
    }

    public override void SetLevelController(LevelController levelController)
    {
        base.SetLevelController(levelController);
        Vector2Int location = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        if (levelController.RoadDict.ContainsKey(location))
        {
            levelController.RoadDict[location] = this;
        }
        else
        {
            levelController.RoadDict.Add(location, this);
        }
    }
}
