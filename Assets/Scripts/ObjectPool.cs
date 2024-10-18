using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public Dictionary<Vector2Int, Grid> ActiveGrid { get; set; } = new Dictionary<Vector2Int, Grid>();
    public Dictionary<Vector2Int, List<Grid>> InactiveGrid { get; set; } = new Dictionary<Vector2Int, List<Grid>>();

    public List<Passenger> ActivePassenger { get; set; } = new List<Passenger>();
    public List<Passenger> InactivePassenger { get; set;} = new List<Passenger>();

    private void Awake()
    {
        if(Instance == null)    
            Instance = this;
    }

    public void AddToActiveGrid<T>(Vector2Int vector2Int, GameObject pref, Transform parent, List<T> list, Vector3 location) where T : GridRoad
    {
        //Debug.Log("Called");
        if (ActiveGrid.ContainsKey(vector2Int))
        {
            if (ActiveGrid[vector2Int] != null && typeof(T) == ActiveGrid[vector2Int].GetType())
            {
                //list.Add(ActiveGrid[vector2Int] as T);
                ActiveGrid[vector2Int].OnStart();
            }
            else
            {
                if(!GetFromInactiveGrid<T>(vector2Int))
                {
                    AddToInactiveGrid<T>(vector2Int);
                    ActiveGrid[vector2Int] = InstantiateGrid<T>(vector2Int, pref, parent, list, location);
                }
                else
                {
                    list.Add(ActiveGrid[vector2Int] as T);
                }
            }
        }
        else
        {
            ActiveGrid.Add(vector2Int, InstantiateGrid<T>(vector2Int, pref, parent, list, location));
        }
    } 

    public void AddToInactiveGrid<T>(Vector2Int vector2Int) where T : GridRoad
    {
        if (ActiveGrid.ContainsKey(vector2Int))
        {
            Grid g = ActiveGrid[vector2Int];
            if (g != null)
            {
                g.ResetParameter();
                ActiveGrid[vector2Int] = null;

                foreach (Grid grid in InactiveGrid[vector2Int])
                {
                    if (typeof(T) == grid.GetType())
                    {

                        return;
                    }
                }

                InactiveGrid[vector2Int].Add(g);
                g.UnActiveSelf();
            }
        }
    }

    public void AddToInactiveGrid(Vector2Int vector2Int) 
    {
        if (ActiveGrid.ContainsKey(vector2Int))
        {
            Grid g = ActiveGrid[vector2Int];
            g.ResetParameter();

            ActiveGrid[vector2Int] = null;

            foreach (Grid grid in InactiveGrid[vector2Int])
            {
                if (g.GetType() == grid.GetType())
                {
                    g.UnActiveSelf();
                    return;
                }
            }

            InactiveGrid[vector2Int].Add(g);
            g.UnActiveSelf();
        }
    }

    bool GetFromInactiveGrid<T>(Vector2Int vector2Int) where T : GridRoad
    {
        foreach (Grid grid in InactiveGrid[vector2Int])
        {
            if (typeof(T) == grid.GetType())
            {
                //Debug.Log(grid.GetSpawnPoint());
                SetActiveGrid(grid);
                grid.OnStart();
                ActiveGrid[vector2Int] = grid;
                InactiveGrid[vector2Int].Remove(grid);

                return true;
            }
        }

        return false;
    }  

    public void InactiveAllActiveGrid()
    {
        List<Vector2Int> keys = ActiveGrid.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            Vector2Int vector2Int = keys[i];
     
            Grid g = ActiveGrid[vector2Int];
            if(g != null)
            {
                g.ResetParameter();
                ActiveGrid[vector2Int] = null;
                g.UnActiveSelf();
                bool check = false;
                if (InactiveGrid.ContainsKey(vector2Int))
                {
                    //Debug.Log(vector2Int);
                    foreach (Grid grid in InactiveGrid[vector2Int])
                    {
                        if (g.GetType() == grid.GetType())
                        {
                            check = true;

                            break;
                        }
                    }

                    if (!check)
                    {
                        InactiveGrid[vector2Int].Add(g);
                    }
                }
                else
                {
                    //Debug.Log(2);
                    List<Grid> list = new List<Grid>();
                    list.Add(g);
                    InactiveGrid.Add(vector2Int, list);
                }
            }           
        }
    }

    public void SetActiveGrid(Grid grid)
    {
        grid.gameObject.SetActive(true);
    }

    T InstantiateGrid<T>(Vector2Int vector2Int, GameObject pref, Transform parent, List<T> list, Vector3 location) where T : GridRoad
    {
        GameObject gridObj = Instantiate(pref, location, Quaternion.identity);
       
        T grid = gridObj.GetComponent<T>();
        grid.SetUp(vector2Int, LevelController.Instance);
        list.Add(grid);
        gridObj.transform.parent = parent;
        return grid;
    }

    public void AddToInactivePassenger(Passenger passenger, Transform parent)
    {  
        if (ActivePassenger.Contains(passenger))
        {
            ActivePassenger.Remove(passenger);
            InactivePassenger.Add(passenger);
            passenger.transform.parent = parent;
            passenger.UnActiveSelf();
        }
    }

    public Passenger AddToActivePassenger(GridPassenger gridPassenger, ExitArea exitArea, ColorType colorType, GameObject passengerPref)
    {
        if (InactivePassenger.Count > 0)
        {
            Passenger passenger = InactivePassenger[0];
            ActivePassenger.Add(passenger);
            SetActivePassenger(passenger);
            
            passenger.ResetParameter(exitArea, colorType);
            gridPassenger.Passenger = passenger;
            passenger.GridPassenger = gridPassenger;

            passenger.transform.position = gridPassenger.GetTransformPosition();
            passenger.transform.parent = gridPassenger.transform;
            InactivePassenger.Remove(passenger);

            return passenger;
        }
        else
        {
            //Debug.Log("Dont Had");
            GameObject passengerObj = Instantiate(passengerPref);
            Passenger passenger = passengerObj.GetComponent<Passenger>();
            passenger.ExitArea = exitArea;

            passenger.ColorType = colorType;
            gridPassenger.Passenger = passenger;
            passenger.GridPassenger = gridPassenger;
            passenger.transform.position = gridPassenger.GetTransformPosition();
            passenger.transform.parent = gridPassenger.transform;

            ActivePassenger.Add(passenger);
            return passenger;
        }
    }

    void SetActivePassenger(Passenger passenger)
    {
        passenger.gameObject.SetActive(true);
    }
}
