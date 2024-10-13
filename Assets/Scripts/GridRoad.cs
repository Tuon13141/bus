using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridRoad : Grid
{
    
    [SerializeField] protected Car car;
    public Car Car => car;
    [SerializeField] protected List<Car> crossCars = new List<Car>();
    public void SetUp(Vector2Int location, LevelController levelController)
    {
        this.spawnPoint = location;
        this.levelController = levelController;

        if (levelController.GridDict.ContainsKey(location)) 
        {
            //Debug.Log(1);
            levelController.GridDict[location] = this;
        }
        else
        {
            //Debug.Log(2);
            levelController.GridDict.Add(location, this);
        }

    }

    public void SetCar(Car car)
    {
        this.car = car;
    }

    public void AddCrossCar(Car c)
    {
        crossCars.Add(c);
    }

    public bool IsHadCar(Car car)
    {
        if(this.car == null || this.car == car) return false;  
        return true;
    }

    public bool IsHadCar()
    {
        if(car == null) return false;   
        return true;
    }

    public bool IsHadCrossCar()
    {
        if(crossCars.Count > 0) return true;
        return false;
    }

    public void RemoveCar(Car car)
    {
      
        if (this.car == car)
        {
            //Debug.Log("Deleted Car from Road " + transform.position);
            this.car = null;
        }
           
    }

    public void RemoveCrossCar(Car car)
    {
        if (crossCars.Contains(car))
        {
            crossCars.Remove(car);
        }

    }

}
