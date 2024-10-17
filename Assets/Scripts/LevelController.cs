using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class LevelController : MonoBehaviour, IOnStart
{
    public static LevelController Instance;

    [SerializeField] GameManager gameManager;

    [SerializeField] Transform roadParent;
    public Transform RoadParent => roadParent;

    [SerializeField] Transform passengerParent;
    public Transform PassengerParent => passengerParent;

    [SerializeField] Transform borderRoadParent;
    public Transform BorderRoadParent => borderRoadParent;

    [SerializeField] LevelRenderer levelRenderer;
    [SerializeField] Dictionary<Vector2Int, Grid> gridDict = new Dictionary<Vector2Int, Grid>();
    public Dictionary<Vector2Int, Grid> GridDict => gridDict;
    public Car CurrentCar { get; set; }
    GridMainRoad currentMainRoad;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public void OnStart()
    {
        ResetParameter();
    }

    public bool CheckRoad(Vector2Int startPosition, Vector2Int moveDirection, Car car)
    {
        Vector2Int nextGridRoad = startPosition + moveDirection;
        CurrentCar = car;
        bool isMoveCross = false;
        if((moveDirection.x + moveDirection.y) % 2 == 0)
        {
            isMoveCross = true;
        }

        if (gridDict.ContainsKey(nextGridRoad))
        {
            car.AddToMovePoints(nextGridRoad);
            if (gridDict[nextGridRoad] is GridMainRoad)
            {
                //Debug.Log("Find Main Road");
                currentMainRoad = (GridMainRoad)gridDict[nextGridRoad];
                ShowArrowExitArea(true);
                return true;
            }
            else if (gridDict[nextGridRoad] is GridBorderRoad)
            {
                //Debug.Log("Find Border Road");
                var result = FindShortestPathToMainRoad((GridBorderRoad)gridDict[nextGridRoad]);

                if (result.Item1 == null)
                {
                    Debug.Log("Lose !");
                    return false;
                }
                car.AddRangeToMovePoints(result.Item1);
                car.RoadOptional(result.Item2);
                return true;
            }
            else if (gridDict[nextGridRoad] is GridRoad)
            {
                //Debug.Log("Find Road");
                GridRoad gridRoad = gridDict[nextGridRoad] as GridRoad;
                if (gridRoad.IsHadCar(car))
                {
                    Debug.Log("Find Car " + nextGridRoad);
                    return false;
                }

                if (isMoveCross)
                {
                    if (gridRoad.IsHadCrossCar())
                    {
                        Debug.Log("Find Car Cross " + nextGridRoad);
                        return false;
                    }
                }
                return CheckRoad(nextGridRoad, moveDirection, car);
            }
            else
            {
                
            }
        }
        else
        {
            
        }

        return false;
    }

    public (List<Vector3> path, List<GridMainRoad> foundMainRoads) FindShortestPathToMainRoad(GridBorderRoad startBorderRoad)
    {
        Queue<(GridBorderRoad currentRoad, List<GridBorderRoad> path)> queue = new Queue<(GridBorderRoad, List<GridBorderRoad>)>();

        HashSet<GridBorderRoad> visited = new HashSet<GridBorderRoad>();

        queue.Enqueue((startBorderRoad, new List<GridBorderRoad> { startBorderRoad }));
        visited.Add(startBorderRoad);

        while (queue.Count > 0)
        {
            var (currentRoad, path) = queue.Dequeue();

            if (currentRoad.MainRoad != null)
            {
                List<GridMainRoad> list = new List<GridMainRoad>();
                list.Add(currentRoad.MainRoad);
                currentMainRoad = currentRoad.MainRoad;
                return (ConvertPathToPositions(path), list);
            }

            foreach (GridBorderRoad neighbor in GetNeighbors(currentRoad))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    List<GridBorderRoad> newPath = new List<GridBorderRoad>(path);
                    newPath.Add(neighbor);

                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        return (null, null);
    }

    public (List<Vector3> path, List<GridExitEnterRoad> foundExitEnterRoads) FindShortestPathToExitEnterRoad(GridMainRoad startRoad, ExitArea exitArea)
    {
        Queue<(GridMainRoad currentRoad, List<GridMainRoad> path)> queue = new Queue<(GridMainRoad, List<GridMainRoad>)>();
        HashSet<GridMainRoad> visited = new HashSet<GridMainRoad>();

        queue.Enqueue((startRoad, new List<GridMainRoad> { startRoad }));
        visited.Add(startRoad);

        while (queue.Count > 0)
        {
            var (currentRoad, path) = queue.Dequeue();

            if (currentRoad.ExitEnterRoads.Count > 0)
            {
                foreach(GridExitEnterRoad exit in currentRoad.ExitEnterRoads)
                {
                    if (!exit.ExitStopRoad.IsHadCar() && exit.ExitStopRoad.IsOpen && exit.ExitArea == exitArea)
                    {
                        return (ConvertPathToPositions(path), currentRoad.ExitEnterRoads);
                    }
                }
                
            }

            foreach (GridMainRoad neighbor in GetNeighbors(currentRoad))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    List<GridMainRoad> newPath = new List<GridMainRoad>(path);
                    newPath.Add(neighbor);

                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        return (null, null);
    }

    public List<Vector3> FindShortestPathToExitMainRoad(GridMainRoad startRoad)
    {
        Queue<(GridMainRoad currentRoad, List<GridMainRoad> path)> queue = new Queue<(GridMainRoad, List<GridMainRoad>)>();

        HashSet<GridMainRoad> visited = new HashSet<GridMainRoad>();

        queue.Enqueue((startRoad, new List<GridMainRoad> { startRoad }));
        visited.Add(startRoad);

        // Bắt đầu duyệt
        while (queue.Count > 0)
        {
            var (currentRoad, path) = queue.Dequeue();

            if (currentRoad.GetIsExitRoad())
            {
                return ConvertPathToPositions(path); 
            }

            foreach (GridMainRoad neighbor in GetNeighbors(currentRoad))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    List<GridMainRoad> newPath = new List<GridMainRoad>(path);
                    newPath.Add(neighbor);

                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        return null;
    }
    private List<T> GetNeighbors<T>(T currentRoad) where T : INeighborable<T>
    {
        List<T> neighbors = new List<T>();

        if (currentRoad.Up != null) neighbors.Add(currentRoad.Up);
        if (currentRoad.Bot != null) neighbors.Add(currentRoad.Bot);
        if (currentRoad.Left != null) neighbors.Add(currentRoad.Left);
        if (currentRoad.Right != null) neighbors.Add(currentRoad.Right);

        return neighbors;
    }

    private List<Vector3> ConvertPathToPositions<T>(List<T> path) where T : Grid
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (T road in path)
        {
            Vector2Int spawnPoint = road.GetSpawnPoint(); 
            Vector3 position = new Vector3(spawnPoint.x, 0, spawnPoint.y); 
            positions.Add(position);
        }

        return positions;
    }


    public GridMainRoad FindNearestMainRoad(Vector2Int currentPosition)
    {
        GridMainRoad nearestRoad = null;
        float nearestDistance = float.MaxValue;

        foreach (var road in GridDict)
        {
            if (road.Value is GridMainRoad mainRoad)
            {
                float distance = Vector2Int.Distance(currentPosition, mainRoad.GetSpawnPoint());
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestRoad = mainRoad;
                }
            }
        }

        //Debug.Log(nearestRoad);
        return nearestRoad; 
    }

    public void RemovePassengerAndShift(GridPassenger gridPassenger, ExitArea exitArea)
    {
        //Debug.Log("Controller");

        gridPassenger.Passenger = null;

        GridPassenger currentGridPassenger = gridPassenger.previousGridPassenger;
        GridPassenger nextGridPassenger = gridPassenger;

        while (currentGridPassenger != null)
        {
            if (currentGridPassenger.IsHadPassenger())
            {
                Passenger passenger = currentGridPassenger.Passenger;
                nextGridPassenger.Passenger = passenger;
                passenger.MovePoints.Add(nextGridPassenger.GetTransformPosition());
                passenger.transform.parent = nextGridPassenger.transform;
                passenger.Move();

                nextGridPassenger.Passenger.GridPassenger = nextGridPassenger;

                currentGridPassenger.Passenger = null;

                nextGridPassenger = currentGridPassenger;

             
            }
          

            currentGridPassenger = currentGridPassenger.previousGridPassenger;
        }

        StartCoroutine(exitArea.InstantiatePassengers());
        //CarInGridExitStayRoadGetPassenger();
    }

    public void CarInGridExitStayRoadGetPassenger(ExitArea exitArea)
    {
        foreach (GridExitStopRoad gridExitStopRoad in exitArea.GridExitStopRoads)
        {
            if (gridExitStopRoad.IsHadCar())
            {
                gridExitStopRoad.Car.ChangeStat(CarStat.OnExitRoad);
            }
        }
    }

    public void ShowArrowExitArea(bool b)
    {
        if (levelRenderer.ExitAreas.Count == 1 && b)
        {
            ChoicedExitErea(levelRenderer.ExitAreas[0]);
            return;
        }

        List<ExitArea> exitAreas = new List<ExitArea>();
        exitAreas.AddRange(levelRenderer.ExitAreas);
        foreach (ExitArea exitArea in levelRenderer.ExitAreas)
        {
            if ((exitArea.IsFull && exitArea.PassengerList.Count <= 0) || !exitArea.HadEmptyExitStay())
            {
                exitAreas.Remove(exitArea);
            }
        }

        if (exitAreas.Count > 1)
        {
            foreach (ExitArea exitArea in levelRenderer.ExitAreas)
            {
                exitArea.ShowArrow(b);
            }
        }
        else if(exitAreas.Count == 1 && b) 
        {
            ChoicedExitErea(exitAreas[0]);
        }
        else
        {
            //Debug.Log("Null");
        }
    }

    public void ChoicedExitErea(ExitArea exitArea)
    {
        var result = FindShortestPathToExitEnterRoad(currentMainRoad, exitArea);

        if (result.Item1 == null)
        {
            Debug.Log("Lose !");
            return;
        }

        CurrentCar.AddRangeToMovePoints(result.Item1);
        CurrentCar.RoadOptional(result.Item2);

        ShowArrowExitArea(false);
    }

    public void SetLevelRenderer(LevelRenderer levelRenderer)
    {
        this.levelRenderer = levelRenderer;
    }

    public void ResetParameter()
    {
        //RemoveNullInGridDict();
        //Debug.Log(GridDict.Keys.Count);
        //foreach (Vector2Int grid in GridDict.Keys)
        //{
        //    Grid g = GridDict[grid];
        //    if (g.DestroyOnNewLoad)
        //    {
        //        g.DestroySelf();
        //    }
        //}
        GridDict.Clear();
        currentMainRoad = null;
        CurrentCar = null;
    }

    public void CheckLevelCompletedCondition(bool checkWin = false)
    {
        foreach (ExitArea exitArea in levelRenderer.ExitAreas) 
        { 
            if((exitArea.PassengerList.Count > 0 && exitArea.IsFull && exitArea.HadEmptyExitStay()) 
                || (exitArea.HadEmptyExitStay() && !exitArea.IsFull) )
            {
                return;
            }

            if (exitArea.IsStuck())
            {
                ObjectPool.Instance.InactiveAllActiveGrid();
                gameManager.Lose();
                return;
            }
        }

        if (checkWin)
        {
            ObjectPool.Instance.InactiveAllActiveGrid();
            gameManager.Win();
        }
    
    }

    public void CheckLevelFailedCondition()
    {
        ObjectPool.Instance.InactiveAllActiveGrid();
        gameManager.Lose();
    }

    public void RemoveNullInGridDict()
    {
        List<Vector2Int> keysToRemove = gridDict.Where(kvp => kvp.Value == null)
                                   .Select(kvp => kvp.Key)
                                   .ToList();

        foreach (Vector2Int key in keysToRemove)
        {
            gridDict.Remove(key);
        }
    }
}
