using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPassenger : Grid
{
    public Passenger Passenger { get; set; } 

    public GridPassenger previousGridPassenger;
    public GridPassenger nextGridPassenger;

    public bool IsStartPoint;

    [SerializeField] List<GridExitStopRoad> connectedGridExitStayGrids;
    
    public bool IsHadPassenger()
    {
        if(this.Passenger == null) return false;
        return true;
    }

    public override void OnStart()
    {
        base.OnStart();
       
    }

    public override void SetLevelController(LevelController levelController)
    {
        base.SetLevelController(levelController);
        Vector2Int location = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        if (levelController.GridDict.ContainsKey(location))
        {
            levelController.GridDict[location] = this;
        }
        else
        {
            levelController.GridDict.Add(location, this);
        }
    }
}
