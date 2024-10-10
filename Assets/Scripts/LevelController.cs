using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    [SerializeField] LevelRenderer levelRenderer;
    [SerializeField] Dictionary<Vector2Int, GridRoad> roadDict = new Dictionary<Vector2Int, GridRoad>();
    public Dictionary<Vector2Int, GridRoad> RoadDict => roadDict;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    public bool CheckRoad(Vector2Int startPosition, Vector2Int moveDirection, Car car)
    {
        Vector2Int nextGridRoad = startPosition + moveDirection;

        if (roadDict.ContainsKey(nextGridRoad))
        {
            car.AddToMovePoints(nextGridRoad);
            if (roadDict[nextGridRoad] is GridMainRoad)
            {
                //Debug.Log("Find Main Road");
                return true;
            }
            else if (roadDict[nextGridRoad] is GridBorderRoad)
            {
                //Debug.Log("Find Border Road");
                car.AddRangeToMovePoints(FindShortestPathToMainRoad((GridBorderRoad) roadDict[nextGridRoad]));
                return true;
            }
            else if (roadDict[nextGridRoad] is GridRoad)
            {
                //Debug.Log("Find Road");
                if (RoadDict[nextGridRoad].IsHadCar(car))
                {
                    Debug.Log("Find Car " + nextGridRoad);
                    return false;
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

    public List<Vector3> FindShortestPathToMainRoad(GridBorderRoad startBorderRoad)
    {
        Queue<(GridBorderRoad currentRoad, List<GridBorderRoad> path)> queue = new Queue<(GridBorderRoad, List<GridBorderRoad>)>();

        HashSet<GridBorderRoad> visited = new HashSet<GridBorderRoad>();

        queue.Enqueue((startBorderRoad, new List<GridBorderRoad> { startBorderRoad }));
        visited.Add(startBorderRoad);

        while (queue.Count > 0)
        {
            var (currentRoad, path) = queue.Dequeue();

            if (currentRoad.MainRoads.Count > 0)
            {
                return ConvertPathToPositions(path);
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

        return null;
    }

    List<GridBorderRoad> GetNeighbors(GridBorderRoad currentRoad)
    {
        List<GridBorderRoad> neighbors = new List<GridBorderRoad>();

        if (currentRoad.UpBorderRoad != null) neighbors.Add(currentRoad.UpBorderRoad);
        if (currentRoad.BotBorderRoad != null) neighbors.Add(currentRoad.BotBorderRoad);
        if (currentRoad.LeftBorderRoad != null) neighbors.Add(currentRoad.LeftBorderRoad);
        if (currentRoad.RightBorderRoad != null) neighbors.Add(currentRoad.RightBorderRoad);

        return neighbors;
    }

    List<Vector3> ConvertPathToPositions(List<GridBorderRoad> path)
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (GridBorderRoad road in path)
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

        foreach (var road in RoadDict)
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

        Debug.Log(nearestRoad);
        return nearestRoad; 
    }
}
