using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class LevelRenderer : MonoBehaviour
{
    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;
    [SerializeField] List<ExitArea> exitAreas;
    public List<ExitArea> ExitAreas => exitAreas;
    [SerializeField] GameObject exitAreaRoadPref;



    [SerializeField] List<GridMainRoad> gridMainRoadList = new List<GridMainRoad>();

    [SerializeField] Vector2Int levelArea;

    [SerializeField] List<Vector2Int> exitAreaSizes;

    [SerializeField] GameObject exitRoadParent;

    [SerializeField] GameObject roadParent;
    public List<GridRoad> Roads { get; set; } = new List<GridRoad>();

    [SerializeField] GameObject mainRoadParent;
    public List<GridMainRoad> MainRoads =>  gridMainRoadList;

    [SerializeField] GameObject borderRoadParent;
    public List<GridBorderRoad> BorderRoads { get; set; } = new List<GridBorderRoad>();



    [SerializeField] List<Car> carList = new List<Car>();
    public List<Car> CarList => carList;

    LevelController levelController;
    InputManager inputManager;



    private void Start()
    {
        levelController = LevelController.Instance;
        inputManager = InputManager.Instance;

        levelController.OnStart();
        inputManager.OnStart();

        SetUpMainRoad();
        GenerateRoadGrid();
        SetUpRoadConnections();
        //AdjustCameraToFitGrid();
       
        StartCar();
    }

    void GenerateRoadGrid()
    {
        int halfX = levelArea.x / 2;
        int halfY = levelArea.y / 2;

        inputManager.XLimitMin = - halfX - 5;
        inputManager.XLimitMax = halfX + 5;
        inputManager.ZLimitMin = - halfY - 5;
        inputManager.ZLimitMax = halfY + 5;

        for(int i = -halfX; i < halfX; i++)
        {
            for(int j = -halfY; j < halfY; j++)
            {
                GameObject roadObj;
                Vector2Int spawnPoint = new Vector2Int(i, j);
                Vector3 location = new Vector3(i, -0.3f, j);

                if (levelController.GridDict.ContainsKey(spawnPoint))
                {
                    continue;   
                }
                if (i == -halfX || i == halfX - 1 || j == -halfY || j == halfY - 1)
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

        for(int x = 0; x < exitAreas.Count; x++)
        {
            exitAreas[x].SetLevelController(levelController);
            exitAreas[x].OnStart();
            GameObject exitAreaObject = exitAreas[x].gameObject;
            Vector2Int exitAreaPosition = new Vector2Int((int)exitAreaObject.transform.position.x * 2, (int)exitAreaObject.transform.position.z * 2);
            for (int i = exitAreaPosition.x / 2 - exitAreaSizes[x].x / 2; i < exitAreaPosition.x / 2 + exitAreaSizes[x].x / 2; i++)
            {
                for (int j = exitAreaPosition.y / 2; j < exitAreaPosition.y / 2 + exitAreaSizes[x].y; j++)
                {
                    Vector3 location = new Vector3(i, -0.3f, j);
                    Vector2Int grid = new Vector2Int(i, j);
                    GameObject go = Instantiate(exitAreaRoadPref, location, Quaternion.identity);
                    go.transform.parent = exitRoadParent.transform;
            

                    if (levelController.GridDict.ContainsKey(grid))
                    {
                        Destroy(levelController.GridDict[grid].gameObject);
                        levelController.GridDict.Remove(grid);
                    }
                }
            }
        }
    

        //exitAreaObject.transform.position = new Vector3Int(exitAreaPosition.x / 2, 0, exitAreaPosition.y / 2);
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
                    borderRoad.Up = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoad = (GridMainRoad)gridRoad;
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, -1));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.Bot = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoad = (GridMainRoad)gridRoad;
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(-1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.Left = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoad = (GridMainRoad)gridRoad;
                }
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridBorderRoad)
                {
                    borderRoad.Right = (GridBorderRoad)gridRoad;
                }
                else if (gridRoad is GridMainRoad)
                {
                    borderRoad.MainRoad = (GridMainRoad)gridRoad;
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
                    mainRoad.Up = (GridMainRoad)gridRoad;
                }
         
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(0, -1));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.Bot = (GridMainRoad)gridRoad;
                }
           
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(-1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.Left = (GridMainRoad)gridRoad;
                }
       
            }

            gridRoad = GetNerbyGrid(borderRoadPos, new Vector2Int(1, 0));
            if (gridRoad != null)
            {
                if (gridRoad is GridMainRoad)
                {
                    mainRoad.Right = (GridMainRoad)gridRoad;
                }
  
            }
        }

        foreach(ExitArea area in exitAreas)
        {
            foreach (GridExitEnterRoad gridExitEnterRoad in area.GridExitEnterRoads)
            {
                gridExitEnterRoad.SetLevelController(levelController);
                gridExitEnterRoad.OnStart();

            }
        }    
      

        foreach (ExitArea area in exitAreas)
        {
            foreach (GridExitStopRoad gridExitStopRoad in area.GridExitStopRoads)
            {
                gridExitStopRoad.SetLevelController(levelController);
                gridExitStopRoad.OnStart();

            }
        }
       

        for (int x = 0; x < exitAreas.Count; x++)
        {
            ExitArea exitArea = exitAreas[x];
            foreach (GridPassengerList gridPassengerList in exitArea.GridPassengerListList)
            {
                foreach (GridPassenger gridPassenger in gridPassengerList.gridPassengers)
                {
                    gridPassenger.OnStart();
                }
            }
        }
     
    }

    void SetUpMainRoad()
    {
        foreach(GridMainRoad gridMainRoad in gridMainRoadList)
        {
            gridMainRoad.OnStart();
            gridMainRoad.SetLevelController(levelController);
            Vector2Int spawnPoint = new Vector2Int((int)gridMainRoad.GetSpawnPoint().x, (int)gridMainRoad.GetSpawnPoint().y);
            //Debug.Log(spawnPoint);

            if (levelController.GridDict.ContainsKey(spawnPoint))
            {
                levelController.GridDict[spawnPoint] = gridMainRoad;
            }
            else
            {
                levelController.GridDict.Add(spawnPoint, gridMainRoad);
            }
        }
    }
    Grid GetNerbyGrid(Vector2Int direction, Vector2Int startPoint)
    {
        if (levelController.GridDict.ContainsKey(direction + startPoint))
        {
            if(levelController.GridDict[direction + startPoint] is GridRoad)
            {
                Grid gridRoad = (Grid)levelController.GridDict[direction + startPoint];
                return gridRoad;
            }
            
        }
       
        return null;
    }

    void StartCar()
    {
        Car[] cars = GetComponentsInChildren<Car>(true);


        carList = new List<Car>(cars);

        foreach (Car car in carList)
        {
            car.OnStart();
        }
    }

    public void InstantiatePassenger(ExitArea exitArea)
    {
        StartCoroutine(exitArea.InstantiatePassengers());
    }
}
