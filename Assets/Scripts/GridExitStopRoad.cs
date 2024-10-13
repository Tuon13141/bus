using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridExitStopRoad : GridRoad
{
    public GridExitEnterRoad GridExitEnterRoad { get; set; }
    public List<GridPassenger> ConnectedGridPassengers { get; set; } = new List<GridPassenger>();

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
        if (levelController.GridDict.ContainsKey(location))
        {
            levelController.GridDict[location] = this;
        }
        else
        {
            levelController.GridDict.Add(location, this);
        }
    }

    public List<Passenger> GetPassenger(ColorType colorType)
    {
        
        List<Passenger> passengers = new List<Passenger>();
        foreach (GridPassenger gridPassenger in ConnectedGridPassengers)
        {
            if(gridPassenger.IsHadPassenger() && gridPassenger.Passenger.ColorType == colorType)
            {
                passengers.Add(gridPassenger.Passenger);
                levelController.RemovePassengerAndShift(gridPassenger);
            }
         
        }

        return passengers;
    }
}
