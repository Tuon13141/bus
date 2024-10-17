using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    [SerializeField] GameObject cornerObject1;
    [SerializeField] GameObject cornerObject2;

    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;
    [SerializeField] List<ExitArea> exitAreas;
    public List<ExitArea> ExitAreas => exitAreas;

    [SerializeField] List<GridMainRoad> gridMainRoadList = new List<GridMainRoad>();

    [SerializeField] Vector2Int cameraStartPosition;

    Transform roadParent;
    public List<GridRoad> Roads { get; set; } = new List<GridRoad>();

    [SerializeField] GameObject mainRoadParent;
    public List<GridMainRoad> MainRoads =>  gridMainRoadList;

    Transform borderRoadParent;
    public List<GridBorderRoad> BorderRoads { get; set; } = new List<GridBorderRoad>();



    [SerializeField] List<Car> carList = new List<Car>();
    public List<Car> CarList => carList;

    LevelController levelController;
    InputManager inputManager;



    private void Start()
    {
        levelController = LevelController.Instance;
        inputManager = InputManager.Instance;

        borderRoadParent = levelController.BorderRoadParent;
        roadParent = levelController.RoadParent;

        levelController.SetLevelRenderer(this);
        levelController.OnStart();
        inputManager.OnStart();

        SetUpMainRoad();
        GenerateRoadGrid();
        SetUpRoadConnections();
        CenterizeCamera();
       
        StartCar();
    }

    void GenerateRoadGrid()
    {
        Vector2Int levelArea = new Vector2Int();
        Vector3 pos1 = cornerObject1.transform.position;
        Vector3 pos2 = cornerObject2.transform.position;
        Vector3 center = (pos1 + pos2) / 2f;

        Vector3 bottomLeft = new Vector3(Mathf.Min(pos1.x, pos2.x), pos1.y, Mathf.Min(pos1.z, pos2.z));
        Vector3 topRight = new Vector3(Mathf.Max(pos1.x, pos2.x), pos1.y, Mathf.Max(pos1.z, pos2.z));


        for (int i = (int)bottomLeft.x; i <= (int)topRight.x; i += 1)
        {
            for (int j = (int)bottomLeft.z; j <= (int)topRight.z; j += 1)
            {
                GameObject roadObj;
                Vector2Int spawnPoint = new Vector2Int(i, j);
                Vector3 location = new Vector3(i, -0.3f, j);

                if (levelController.GridDict.ContainsKey(spawnPoint))
                {
                    continue;   
                }
                if (i == bottomLeft.x || i == topRight.x || j == bottomLeft.z || j == topRight.z)
                {
                    ObjectPool.Instance.AddToActiveGrid<GridBorderRoad>(spawnPoint, borderRoadPref, borderRoadParent, BorderRoads, location);
                }
                else
                {
                    ObjectPool.Instance.AddToActiveGrid<GridRoad>(spawnPoint, roadPref, roadParent, Roads, location);
                }
            }
        }

        for(int x = 0; x < exitAreas.Count; x++)
        {
            exitAreas[x].SetLevelController(levelController);
            exitAreas[x].OnStart();
        
        }

        int width = Mathf.Abs(Mathf.FloorToInt(pos2.x - pos1.x));
        int height = Mathf.Abs(Mathf.FloorToInt(pos2.z - pos1.z));

        levelArea = new Vector2Int(width, height);

        int halfX = levelArea.x / 2;
        int halfY = levelArea.y / 2;

        inputManager.XLimitMin = -halfX - 5;
        inputManager.XLimitMax = halfX + 5;
        inputManager.ZLimitMin = -halfY - 5;
        inputManager.ZLimitMax = halfY + 5;

        cornerObject1.SetActive(false);
        cornerObject2.SetActive(false);
    }
    void CenterizeCamera()
    {
        Camera mainCamera = Camera.main;

        mainCamera.transform.position = new Vector3(cameraStartPosition.x, 60, cameraStartPosition.y);

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
            gridMainRoad.SetLevelController(levelController);
            gridMainRoad.OnStart();
            
            //Vector2Int spawnPoint = new Vector2Int((int)gridMainRoad.GetSpawnPoint().x, (int)gridMainRoad.GetSpawnPoint().y);
            ////Debug.Log(spawnPoint);

            //if (levelController.GridDict.ContainsKey(spawnPoint))
            //{
            //    Destroy(levelController.GridDict[spawnPoint].gameObject);
            //    levelController.GridDict[spawnPoint] = gridMainRoad;
            //}
            //else
            //{
            //    levelController.GridDict.Add(spawnPoint, gridMainRoad);
            //}
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
