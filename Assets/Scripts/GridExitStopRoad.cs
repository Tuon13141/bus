using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExitStopRoad : GridRoad
{
    public GridExitEnterRoad GridExitEnterRoad { get; set; }

    [SerializeField] bool isOpen = false;
    public bool IsOpen => isOpen;
    public override void OnStart()
    {
        base.OnStart();
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
