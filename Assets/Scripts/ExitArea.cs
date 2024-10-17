using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitArea : MonoBehaviour, IOnStart
{
    [SerializeField] GameObject exitAreaRoadPref;
    [SerializeField] GameObject exitRoadParent;
    [SerializeField] GameObject passengerPref;
    [SerializeField] ExitAreaType exitAreaType;
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


    [SerializeField] Vector2Int exitAreaSize;

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
                        GameObject passengerObj = Instantiate(passengerPref);
                        Passenger passenger = passengerObj.GetComponent<Passenger>();
                        passenger.ExitArea = this;

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
                    Vector3 location = new Vector3(i, -0.3f, j);
                    Vector2Int grid = new Vector2Int(i, j);
                    GameObject go = Instantiate(exitAreaRoadPref);

                    go.transform.parent = exitRoadParent.transform;
                    go.transform.localPosition = location;


                    Vector2Int vector2Int = grid + new Vector2Int((int)transform.position.x, (int)transform.position.z);
                    if (levelController.GridDict.ContainsKey(vector2Int))
                    {
                        //Debug.Log(vector2Int);
                        Destroy(levelController.GridDict[vector2Int].gameObject);
                        levelController.GridDict.Remove(vector2Int);
                    }
                    else
                    {
                        //Debug.Log("None");
                    }
                }
            }
        }
        else
        {
            for (int i = -halfX; i < halfX; i++)
            {
                for (int j = 0; j < exitAreaSize.y; j++)
                {
                    Vector3 location = new Vector3(j, -0.3f, i);
                    Vector2Int grid = new Vector2Int(j, i);
                    GameObject go = Instantiate(exitAreaRoadPref);

                    go.transform.parent = exitRoadParent.transform;
                    go.transform.localPosition = location;


                    Vector2Int vector2Int = grid + new Vector2Int((int)transform.position.x, (int)transform.position.z);
                    if (levelController.GridDict.ContainsKey(vector2Int))
                    {
                        //Debug.Log(vector2Int);
                        Destroy(levelController.GridDict[vector2Int].gameObject);
                        levelController.GridDict.Remove(vector2Int);
                    }
                    else
                    {
                        //Debug.Log("None");
                    }
                }
            }
        }
    }

    public bool IsStuck()
    {
        foreach (GridExitStopRoad gridExitStopRoad in gridExitStopRoads)
        {
            if (!gridExitStopRoad.IsHadCar())
            {
                return false;
            }
        }

        if (passengerList.Count > 0)
        {
            return true;
        }
        return false;
    }
}

public enum ExitAreaType
{
    Horizontal, Vertical
}
