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
    [SerializeField] GameObject passengerPref;

    [SerializeField] List<GridExitEnterRoad> gridExitEnterRoads = new List<GridExitEnterRoad>();
    [SerializeField] List<GridExitStopRoad> gridExitStopRoads = new List<GridExitStopRoad>();
    public List<GridExitStopRoad> GridExitStopRoads => gridExitStopRoads;
    [SerializeField] List<GridPassengerList> gridPassengerListList = new();

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

    [SerializeField] List<Passenger> passengerList = new List<Passenger>();

    [SerializeField] List<Car> carList = new List<Car>();
    public List<Car> CarList => carList;

    LevelController levelController;
    InputManager inputManager;

    [SerializeField] List<PassengerWave> passengerWaves = new List<PassengerWave>();

    private void Start()
    {
        levelController = LevelController.Instance;
        inputManager = InputManager.Instance;

        levelController.OnStart();
        inputManager.OnStart();

        GenerateRoadGrid();
        SetUpRoadConnections();
        AdjustCameraToFitGrid();
        StartCoroutine(InstantiatePassengers());
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

                    if(i == halfX - 1)
                    {
                        road.SetIsExitRoad(true);
                    }
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

                if (levelController.GridDict.ContainsKey(grid))
                {
                    Destroy(levelController.GridDict[grid].gameObject);
                    levelController.GridDict.Remove(grid);
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
                    borderRoad.Up = (GridBorderRoad)gridRoad;
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
                    borderRoad.Bot = (GridBorderRoad)gridRoad;
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
                    borderRoad.Left = (GridBorderRoad)gridRoad;
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
                    borderRoad.Right = (GridBorderRoad)gridRoad;
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

        foreach(GridExitEnterRoad gridExitEnterRoad in gridExitEnterRoads)
        {
            gridExitEnterRoad.SetLevelController(levelController);
            gridExitEnterRoad.OnStart();
          
        }

        foreach(GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            gridExitStopRoad.SetLevelController(levelController);
            gridExitStopRoad.OnStart();
           
        }
        
        foreach(GridPassengerList gridPassengerList in gridPassengerListList)
        {
            foreach(GridPassenger gridPassenger in gridPassengerList.gridPassengers)
            {
                gridPassenger.OnStart();
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

    int currentIndexInPassengerWaves = 0;
    int currentIndexOfPassengerWave = 0;
    bool canInstantiatePassengers = true;
    public IEnumerator InstantiatePassengers()
    {
        if(!canInstantiatePassengers) 
        {
            //levelController.CarInGridExitStayRoadGetPassenger();
            yield break; 
        }
        canInstantiatePassengers = false;

        if(currentIndexInPassengerWaves == passengerWaves.Count - 1) 
        {
            levelController.CarInGridExitStayRoadGetPassenger();
            //canInstantiatePassengers = true;
            yield break; 
        }

        for (int i = currentIndexInPassengerWaves; i < passengerWaves.Count; i++)
        {
            currentIndexInPassengerWaves = i;
            int indexInGridPassengers = -1;
            for (int j = currentIndexOfPassengerWave; j < passengerWaves[i].numberOfPassenger; j++)
            {
                indexInGridPassengers++;
                if(indexInGridPassengers >= gridPassengerListList.Count)
                {
                    indexInGridPassengers = 0;
                }
                foreach (GridPassenger gridPassenger in gridPassengerListList[indexInGridPassengers].gridPassengers)
                {
                    if (gridPassenger.IsStartPoint)
                    {
                        if (gridPassenger.IsHadPassenger())
                        {
                            //levelController.CarInGridExitStayRoadGetPassenger();
                            canInstantiatePassengers = true;
                            yield break;
                        }
                        GameObject passengerObj = Instantiate(passengerPref);
                        Passenger passenger = passengerObj.GetComponent<Passenger>();

                        passenger.ColorType = passengerWaves[i].colorType;
                        gridPassenger.Passenger = passenger;
                        passenger.GridPassenger = gridPassenger;
                        passenger.transform.position = gridPassenger.GetTransformPosition();
                        passenger.transform.parent = gridPassenger.transform;

                        GridPassenger nextGridPassenger = gridPassenger.nextGridPassenger;
                        GridPassenger currentGridPassenger = gridPassenger;

                        while (nextGridPassenger != null && !nextGridPassenger.IsHadPassenger())
                        {
                            //Debug.Log(nextGridPassenger.gameObject.name);
                           
                            nextGridPassenger.Passenger = passenger;
                            passenger.GridPassenger = nextGridPassenger;
                            passenger.transform.parent = nextGridPassenger.transform;
                            passenger.MovePoints.Add(nextGridPassenger.GetTransformPosition());

                            currentGridPassenger.Passenger = null;

                            currentGridPassenger = nextGridPassenger;

                            nextGridPassenger = nextGridPassenger.nextGridPassenger;
                        }
                        currentIndexOfPassengerWave = j + 1;
                        passenger.Move();
                        passengerList.Add(passenger);
                        levelController.CarInGridExitStayRoadGetPassenger();
                        break;
                    }
                }

                //Debug.Log(3);
               
                yield return new WaitForSeconds(0.1f);
            }
            //Debug.Log(2);
            currentIndexOfPassengerWave = 0;
        }
        //Debug.Log(1);
        canInstantiatePassengers = true;

    }

}
