using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    public List<Vector2Int> roadGridPositionList = new List<Vector2Int>();
    public List<Vector2Int> borderRoadGridPositionList = new List<Vector2Int>();

    [SerializeField] Vector3Int cornerObject1;
    [SerializeField] Vector3Int cornerObject2;

    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;
    [SerializeField] List<ExitArea> exitAreas;
    public List<ExitArea> ExitAreas => exitAreas;

    [SerializeField] List<GridMainRoad> gridMainRoadList = new List<GridMainRoad>();

    [SerializeField] Vector2Int cameraStartPosition;

    Transform roadParent;
    public List<GridRoad> Roads { get; set; } = new List<GridRoad>();

    [SerializeField] GameObject mainRoadParent;
    public List<GridMainRoad> MainRoads => gridMainRoadList;

    Transform borderRoadParent;
    public List<GridBorderRoad> BorderRoads { get; set; } = new List<GridBorderRoad>();

    [SerializeField] Transform carParent;

    [SerializeField] List<Car> carList;
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
        foreach (Vector2Int vector2Int in borderRoadGridPositionList)
        {
            Vector3 location = new Vector3(vector2Int.x, 0.1f, vector2Int.y);

            if (levelController.GridDict.ContainsKey(vector2Int))
            {
                continue;
            }
            ObjectPool.Instance.AddToActiveGrid<GridBorderRoad>(vector2Int, borderRoadPref, borderRoadParent, BorderRoads, location);
        }

        foreach (Vector2Int vector2Int in roadGridPositionList)
        {
            Vector3 location = new Vector3(vector2Int.x, 0.1f, vector2Int.y);

            if (levelController.GridDict.ContainsKey(vector2Int))
            {
                continue;
            }
            ObjectPool.Instance.AddToActiveGrid<GridRoad>(vector2Int, roadPref, roadParent, Roads, location);
        }

       
        List<Vector2Int> combinedList = roadGridPositionList.Concat(borderRoadGridPositionList).ToList();

        Vector2Int minPoint = GetMinPoint(combinedList);
        Vector2Int maxPoint = GetMaxPoint(combinedList);

        cornerObject1 = new Vector3Int(minPoint.x, 0, minPoint.y);
        cornerObject2 = new Vector3Int(maxPoint.x, 0, maxPoint.y);

        Vector2Int levelArea = new Vector2Int();
        Vector3 pos1 = cornerObject1;
        Vector3 pos2 = cornerObject2;
        Vector3 center = (pos1 + pos2) / 2f;

        Vector3 bottomLeft = new Vector3(Mathf.Min(pos1.x, pos2.x), pos1.y, Mathf.Min(pos1.z, pos2.z));
        Vector3 topRight = new Vector3(Mathf.Max(pos1.x, pos2.x), pos1.y, Mathf.Max(pos1.z, pos2.z));


        for (int x = 0; x < exitAreas.Count; x++)
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

        foreach (ExitArea area in exitAreas)
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
        foreach (GridMainRoad gridMainRoad in gridMainRoadList)
        {
            gridMainRoad.SetLevelController(levelController);
            gridMainRoad.OnStart();
        }
    }
    Grid GetNerbyGrid(Vector2Int direction, Vector2Int startPoint)
    {
        if (levelController.GridDict.ContainsKey(direction + startPoint))
        {
            if (levelController.GridDict[direction + startPoint] is GridRoad)
            {
                Grid gridRoad = (Grid)levelController.GridDict[direction + startPoint];
                return gridRoad;
            }

        }

        return null;
    }

    void StartCar()
    {
        foreach (Car car in carList)
        {
            car.OnStart();
        }
    }

    public void InstantiatePassenger(ExitArea exitArea)
    {
        StartCoroutine(exitArea.InstantiatePassengers());
    }

    public Vector2Int GetMinPoint(List<Vector2Int> positions)
    {
        int minX = positions.Min(pos => pos.x);
        int minY = positions.Min(pos => pos.y);
        return new Vector2Int(minX, minY);
    }

    public Vector2Int GetMaxPoint(List<Vector2Int> positions)
    {
        int maxX = positions.Max(pos => pos.x);
        int maxY = positions.Max(pos => pos.y);
        return new Vector2Int(maxX, maxY);
    }

    public void AddMainRoadObjects(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            obj.transform.parent = mainRoadParent.transform;
            MainRoads.Add(obj.GetComponent<GridMainRoad>());
        }
    }

    public void AddExitAreas(List<GameObject> exitAreas)
    {
        ExitAreas.Clear();
        foreach(GameObject obj in exitAreas)
        {
            ExitArea exitArea = obj.GetComponent<ExitArea>();
            obj.transform.parent = transform;
            this.exitAreas.Add(obj.GetComponent<ExitArea>());

        }
    }

    public void AddCars(List<Car> cars)
    {
        carList.Clear();
        foreach(Car car in cars)
        {
            car.gameObject.transform.parent = carParent;
            CarList.Add(car);
        }
    }
}
