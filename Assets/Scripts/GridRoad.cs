using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRoad : MonoBehaviour
{
    //[SerializeField] GridRoad upperLeftRoad;
    //[SerializeField] GridRoad upperMiddleRoad;
    //[SerializeField] GridRoad upperRightRoad;
    //[SerializeField] GridRoad centerLeftRoad;
    //[SerializeField] GridRoad centerMiddleRoad;
    //[SerializeField] GridRoad centerRightRoad;
    //[SerializeField] GridRoad lowerLeftRoad;
    //[SerializeField] GridRoad lowerMiddleRoad;
    //[SerializeField] GridRoad lowerRightRoad;

    [SerializeField] protected Vector2Int spawnPoint;
    [SerializeField] protected LevelController levelController;
    [SerializeField] protected Car car;

    public void SetUp(Vector2Int location, LevelController levelController)
    {
        this.spawnPoint = location;
        this.levelController = levelController;

        if (levelController.RoadDict.ContainsKey(location)) 
        {
            //Debug.Log(1);
            levelController.RoadDict[location] = this;
        }
        else
        {
            //Debug.Log(2);
            levelController.RoadDict.Add(location, this);
        }

    }

    public void SetCar(Car car)
    {
        this.car = car;
    }

    public bool IsHadCar(Car car)
    {
        if(this.car == null || this.car == car) return false;  
        return true;
    }
}
