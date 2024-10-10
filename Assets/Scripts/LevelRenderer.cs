using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    [SerializeField] GameObject mainRoadPref;
    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;
    [SerializeField] GameObject exitAreaObject;
    [SerializeField] GameObject exitAreaRoadPref;

    [SerializeField] List<GridExitEnterRoad> gridExitEnterRoads = new List<GridExitEnterRoad>();
    [SerializeField] List<GridExitStopRoad> gridExitStopRoads = new List<GridExitStopRoad>();


    [SerializeField] Vector2Int levelArea;
  

    [SerializeField] int mainRoadsZ;
    [SerializeField] Vector2Int exitAreaPosition;
    [SerializeField] Vector2Int exitAreaSize;

    [SerializeField] GameObject exitRoadParent;

    [SerializeField] GameObject roadParent;
    public List<GridRoad> Roads { get; set; } = new List<GridRoad>();

    [SerializeField] GameObject mainRoadParent;
    public List<GridMainRoad> MainRoads { get; set; } = new List<GridMainRoad>();

    [SerializeField] GameObject borderRoadParent;
    public List<GridBorderRoad> BorderRoads { get; set; } = new List<GridBorderRoad>();

    [SerializeField] LevelController levelController;

    private void Start()
    {
        GenerateRoadGrid();
        SetUpRoadConnections();
        AdjustCameraToFitGrid();
        StartCar();
    }

    void GenerateRoadGrid()
    {
        int halfX = levelArea.x / 2;
        int halfY = levelArea.y / 2;

        for(int i = -halfX; i < halfX; i++)
        {
            for(int j = -halfY; j < halfY; j++)
            {
                GameObject roadObj;
                Vector2Int spawnPoint = new Vector2Int(i, j);
                Vector3 location = new Vector3(i, -0.3f, j);

                if (j == (int) mainRoadsZ / 2)
                {
                    roadObj = Instantiate(mainRoadPref, location, Quaternion.identity);
                    roadObj.transform.parent = mainRoadParent.transform;
                    GridMainRoad road = roadObj.GetComponent<GridMainRoad>();
                    road.SetUp(spawnPoint, levelController);
                    MainRoads.Add(road);
                }
                else if (i == -halfX || i == halfX - 1 || j == -halfY || j == halfY - 1)
                {
                    roadObj = Instantiate(borderRoadPref, location, Quaternion.identity);
                    roadObj.transform.parent = borderRoadParent.transform;
                    GridBorderRoad road = roadObj.GetComponent<GridBorderRoad>();
                    road.SetUp(spawnPoint, levelController);
                    BorderRoads.Add(road);
                }
                else
                {
                    roadObj = Instantiate(roadPref, location, Quaternion.identity);
                    roadObj.transform.parent = roadParent.transform;
                    GridRoad road = roadObj.GetComponent<GridRoad>();
                    road.SetUp(spawnPoint, levelController);
                    Roads.Add(road);
                }
            }
        }

        //Debug.Log(levelController.RoadDict.Count);
        //GameObject exitAreaObj = Instantiate(exitAreaObject, new Vector3(exitAreaPosition.x, 0, exitAreaPosition.y), Quaternion.identity);
        
        for (int i = exitAreaPosition.x / 2 - exitAreaSize.x / 2 ; i < exitAreaPosition.x / 2 + exitAreaSize.x / 2; i++)
        {
            for (int j = exitAreaPosition.y / 2 ; j < exitAreaPosition.y / 2 + exitAreaSize.y ; j++)
            {
                Vector3 location = new Vector3(i, -0.3f, j);
                Vector2Int grid = new Vector2Int(i, j);
                Instantiate(exitAreaRoadPref, location, Quaternion.identity).transform.parent = exitRoadParent.transform;

                if (levelController.RoadDict.ContainsKey(grid))
                {
                    Destroy(levelController.RoadDict[grid].gameObject);
                    levelController.RoadDict.Remove(grid);
                }
            }
        }

        exitAreaObject.transform.position = new Vector3Int(exitAreaPosition.x / 2, 0, exitAreaPosition.y / 2);
    }
    void AdjustCameraToFitGrid()
    {
        Camera mainCamera = Camera.main;

        float x = -0.5f;
        if (levelArea.x % 2 != 0) {
            x = -0.5f;
        }
      

        Vector3 gridCenter = new Vector3(x, 0, 0);

        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = levelArea.y / 2f;

            float aspectRatio = mainCamera.aspect;
            float horizontalSize = (levelArea.x / 2f) / aspectRatio;
            if (horizontalSize > mainCamera.orthographicSize)
            {
                mainCamera.orthographicSize = horizontalSize;
            }

            mainCamera.transform.position = new Vector3(gridCenter.x, 10, gridCenter.z);  
        }
        else
        {
            float halfHeight = levelArea.y / 2f;
            float halfWidth = levelArea.x / 2f;

            float fovInRadians = mainCamera.fieldOfView * Mathf.Deg2Rad;

            float cameraDistanceVertical = halfHeight / Mathf.Tan(fovInRadians / 2f);

            float aspectRatio = mainCamera.aspect;
            float cameraDistanceHorizontal = halfWidth / (Mathf.Tan(fovInRadians / 2f) * aspectRatio);
            float cameraDistance = Mathf.Max(cameraDistanceVertical, cameraDistanceHorizontal);

            mainCamera.transform.position = new Vector3(gridCenter.x, cameraDistance + 5, gridCenter.z);
        }

        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void SetUpRoadConnections()
    {
        for (int i = 0; i < BorderRoads.Count; i++)
        {
            GridBorderRoad borderRoad = BorderRoads[i];
            Vector2Int borderRoadPos = borderRoad.GetSpawnPoint();

            var gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, 1));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.UpBorderRoad = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoads.Add((GridMainRoad)gridRoad);
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, -1));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.BotBorderRoad = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoads.Add((GridMainRoad)gridRoad);
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(-1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.LeftBorderRoad = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoads.Add((GridMainRoad)gridRoad);
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.RightBorderRoad = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoads.Add((GridMainRoad)gridRoad);
                }
            }


        }

        for (int i = 0; i < MainRoads.Count; i++)
        {
            GridMainRoad mainRoad = MainRoads[i];
            Vector2Int borderRoadPos = mainRoad.GetSpawnPoint();

            var gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, 1));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.UpMainRoad = (GridMainRoad)gridRoad;
                }
         
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, -1));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.BotMainRoad = (GridMainRoad)gridRoad;
                }
           
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(-1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.LeftMainRoad = (GridMainRoad)gridRoad;
                }
       
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.RightMainRoad = (GridMainRoad)gridRoad;
                }
  
            }
        }

        foreach(GridExitEnterRoad gridExitEnterRoad in gridExitEnterRoads)
        {
            gridExitEnterRoad.OnStart();
        }

        foreach(GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            gridExitStopRoad.OnStart();
        }
    }

    GridRoad GetNerbyGrid(Vector2Int direction, Vector2Int startPoint)
    {
        if (levelController.RoadDict.ContainsKey(direction + startPoint)) return levelController.RoadDict[direction + startPoint];
        return null;
    }

    void StartCar()
    {
        Car[] cars = GetComponentsInChildren<Car>(true);


        List<Car> carList = new List<Car>(cars);

        foreach (Car car in carList)
        {
            car.OnStart();
        }
    }
}
