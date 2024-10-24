using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : MonoBehaviour, IOnStart
{
    [SerializeField] GameObject exitAreaRoadPref;
    [SerializeField] GameObject exitRoadParent;
    [SerializeField] GameObject passengerPref;
    [SerializeField] ExitAreaType exitAreaType;
    public ExitAreaType ExitAreaType => exitAreaType;
    [SerializeField] AreaArrowButton arrow;
    [SerializeField] List<GridPassengerList> gridPassengerListList = new();
    public List<GridPassengerList> GridPassengerListList => gridPassengerListList;
    [SerializeField] List<PassengerWave> passengerWaves = new List<PassengerWave>();
    [SerializeField] List<Passenger> passengerList = new List<Passenger>();
    public List<Passenger> PassengerList => passengerList;  

    [SerializeField] List<GridExitEnterRoad> gridExitEnterRoads = new List<GridExitEnterRoad>();
    public List<GridExitEnterRoad> GridExitEnterRoads => gridExitEnterRoads;
    [SerializeField] List<GridExitStopRoad> gridExitStopRoads = new List<GridExitStopRoad>();
    public List<GridExitStopRoad> GridExitStopRoads => gridExitStopRoads;

    public List<Car> CarInExitStops { get; set; } = new List<Car>();

    int currentIndexInPassengerWaves = 0;
    int currentIndexOfPassengerWave = 0;
    bool canInstantiatePassengers = true;
    bool isFull = false;
    public bool IsFull => isFull;   

    LevelController levelController;
    public GridExitArea GridExitArea { get; set; }

    [SerializeField] Vector2Int exitAreaSize;
    public Vector2Int ExitAreaSize => exitAreaSize;

    public void SetLevelController(LevelController levelController)
    {
        this.levelController = levelController;
    }
    public void OnStart()
    {
        arrow.ExitArea = this;
        foreach (GridExitEnterRoad road in gridExitEnterRoads)
        {
            road.ExitArea = this;
        }
        SpawnExitRoadGrid();
        ShowArrow(false);
        StartCoroutine(InstantiatePassengers());
    }
    public IEnumerator InstantiatePassengers()
    {
        //Debug.Log("Called");
        if (!canInstantiatePassengers)
        {

            yield break;
        }
        canInstantiatePassengers = false;
        float time = 0.1f;

        if (currentIndexInPassengerWaves >= passengerWaves.Count)
        {

            CarInGridExitStayRoadGetPassenger();

            yield break;
        }

        for (int i = currentIndexInPassengerWaves; i < passengerWaves.Count; i++)
        {
            currentIndexInPassengerWaves = i;

            int indexInGridPassengers = -1;
            int countStartPointFull = 0;
            for (int j = currentIndexOfPassengerWave; j < passengerWaves[i].numberOfPassenger; j++)
            {
                indexInGridPassengers++;
                if (indexInGridPassengers >= gridPassengerListList.Count)
                {
                    indexInGridPassengers = 0;
                }
                foreach (GridPassenger gridPassenger in gridPassengerListList[indexInGridPassengers].gridPassengers)
                {
                    if (gridPassenger.IsStartPoint)
                    {
                        if (gridPassenger.IsHadPassenger())
                        {
                            countStartPointFull++;
                            if (countStartPointFull >= gridPassengerListList.Count)
                            {
                                canInstantiatePassengers = true;
                                CarInGridExitStayRoadGetPassenger();

                                yield break;
                            }

                            canInstantiatePassengers = true;
                            break;
                        }
                        if (currentIndexInPassengerWaves == passengerWaves.Count - 1 && isFull)
                        {
                            time = 0;
                            break;
                        }
                        Passenger passenger = ObjectPool.Instance.AddToActivePassenger(gridPassenger, this, passengerWaves[i].colorType, passengerPref);

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
                        currentIndexOfPassengerWave += 1;
                        //j++;
                        passenger.Move();
                        passengerList.Add(passenger);

                        break;
                    }
                }

                //Debug.Log(3);

                yield return new WaitForSeconds(time);
            }
            //Debug.Log(2);
            
            currentIndexOfPassengerWave = 0;
        }

        isFull = true;

        canInstantiatePassengers = true;
        CarInGridExitStayRoadGetPassenger();
    }

    public void CarInGridExitStayRoadGetPassenger()
    {
        List<ColorType> colorTypes = new List<ColorType>();
        for (int i = 0; i < CarInExitStops.Count; i++)
        {
            Car car = CarInExitStops[i];
            if (!colorTypes.Contains(car.ColorType))
            {
                car.ChangeStat(CarStat.OnExitRoad);
                colorTypes.Add(car.ColorType);
            }
        }
    }

    public void ShowArrow(bool b)
    {
        arrow.gameObject.SetActive(b);
    }

    public void OnArrowClick()
    {
        levelController.ChoicedExitErea(this);
    }

    public void SpawnExitRoadGrid()
    {
        int halfX = exitAreaSize.x / 2;
        int halfY = exitAreaSize.y / 2;

        if (exitAreaType == ExitAreaType.Horizontal)
        {
            for (int i = -halfX; i < halfX; i++)
            {
                for (int j = 0; j < exitAreaSize.y; j++)
                {
                    Vector3 location = new Vector3(i, -0f, j);
                    Vector2Int grid = new Vector2Int(i, j);
                    //GameObject go = Instantiate(exitAreaRoadPref);

                    //go.transform.parent = exitRoadParent.transform;
                    //go.transform.localPosition = location;


                    Vector2Int vector2Int = grid + new Vector2Int((int)transform.position.x, (int)transform.position.z);
                    //ObjectPool.Instance.AddToInactiveGrid(vector2Int);
                }
            }
        }
        else
        {
            for (int i = -halfX; i < halfX; i++)
            {
                for (int j = 0; j < exitAreaSize.y; j++)
                {
                    Vector3 location = new Vector3(j, -0f, i);
                    Vector2Int grid = new Vector2Int(j, i);
                    //GameObject go = Instantiate(exitAreaRoadPref);

                    //go.transform.parent = exitRoadParent.transform;
                    //go.transform.localPosition = location;


                    Vector2Int vector2Int = grid + new Vector2Int((int)transform.position.x, (int)transform.position.z);
                    //ObjectPool.Instance.AddToInactiveGrid(vector2Int);
                }
            }
        }
    }

    public bool IsStuck()
    {
        foreach (GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            if ((!gridExitStopRoad.IsHadCar() || gridExitStopRoad.Car.CanGetPassenger) && gridExitStopRoad.IsOpen)
            {
                return false;
            }
        }

        return true;
    }

    public void AddCar(Car car)
    {
        CarInExitStops.Add(car);
        foreach (GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            if (!gridExitStopRoad.IsOpen) continue;
            if (!gridExitStopRoad.IsHadCar()) return;
            if (gridExitStopRoad.Car.CanGetPassenger) return;
        }

        levelController.CheckLevelFailedCondition();
    }

    public bool HadEmptyExitStay()
    {
        foreach (GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            if (!gridExitStopRoad.IsHadCar() && gridExitStopRoad.IsOpen)
            {
                return true;
            }
        }

        return false;
    }
}

public enum ExitAreaType
{
    Horizontal, Vertical
}
