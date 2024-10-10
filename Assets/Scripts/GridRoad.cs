using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridRoad : MonoBehaviour
{
    [SerializeField] protected Vector2Int spawnPoint;
    [SerializeField] protected LevelController levelController;
    [SerializeField] protected Car car;
    [SerializeField] protected bool canSpawnCar = true;
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

    public void SetCanSpawnCar(bool b)
    {
        this.canSpawnCar = b;
    }

    public bool IsHadCar(Car car)
    {
        if(this.car == null || this.car == car) return false;  
        return true;
    }

    public void RemoveCar(Car car)
    {
      
        if (this.car == car)
        {
            Debug.Log("Deleted Car from Road " + transform.position);
            this.car = null;
        }
           
    }

    public Vector2Int GetSpawnPoint()
    {
        return spawnPoint;
    }

    public virtual void OnStart()
    {

    }
}
