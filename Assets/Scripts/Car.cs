using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Car : MonoBehaviour, IChangeStat, IOnStart
{
    [SerializeField] int index;
    [SerializeField] Vector2Int scale;
    [SerializeField] int seatCount;
    [SerializeField] CarStat stat = CarStat.OnRoad;
    [SerializeField] ColorType colorType = ColorType.Red;
    public ColorType ColorType => colorType;
    [SerializeField] DirectionType directionType;
    private float rotationDegrees; 
    [SerializeField] Vector3Int spawnPoint;
    [SerializeField] float moveSpeed = 10f;
    public Vector3 originalScale;

    [SerializeField] List<Vector3> movePoints = new List<Vector3>();
    
    public List<Vector3> MovePoints => movePoints;



    [SerializeField] List<Seat> seats= new List<Seat>();
    [SerializeField] List<GameObject> needToHideObjects = new List<GameObject>();

    [SerializeField] Animation carShakingAnimation;
    [SerializeField] List<MeshRenderer> carRenderers = new List<MeshRenderer>();    


    List<Vector2Int> gridHolders = new List<Vector2Int>();
    List<Vector2Int> gridCrossHolders = new List<Vector2Int>();
    List<Passenger> passengers = new List<Passenger>();


    bool isMoving = false;
    Coroutine moveCoroutine = null;
    [SerializeField] LevelController levelController;
    Action clickedAction;
    [SerializeField] GridExitStopRoad gridExitStopRoad;
    public void OnStart()
    {
        //Debug.Log(gameObject.name);
        levelController = LevelController.Instance;
        transform.position = spawnPoint;
        originalScale = transform.localScale;
        movePoints.Clear();
        seatCount = seats.Count;
        //movePoints.Add(spawnPoint);
        
        GetRotation();
        GetColor();
        CalculateOverlappingGrids();  
    }

    void GetRotation()
    {
        switch (directionType)
        {
            case DirectionType.Up:
                rotationDegrees = 0;
                break;
            case DirectionType.Down:
                rotationDegrees = 180;
                break;
            case DirectionType.Left:
                rotationDegrees = 270;
                break;
            case DirectionType.Right:
                rotationDegrees = 90;
                break;
            case DirectionType.UpLeft:
                rotationDegrees = 315;
                break;
            case DirectionType.DownLeft:
                rotationDegrees = 225;
                break;
            case DirectionType.UpRight:
                rotationDegrees = 45;
                break;
            case DirectionType.DownRight:
                rotationDegrees = 135;
                break;
        }

        transform.rotation = Quaternion.Euler(0, rotationDegrees, 0);
    }
    void GetColor()
    {
        switch (colorType)
        {
            case ColorType.Red:
                SetColor(Color.red);
                break;
            case ColorType.Green:
                SetColor(Color.green);
                break;
            case ColorType.Blue:
                SetColor(Color.blue);
                break;
        }
    }

    void SetColor(Color newColor)
    {
        foreach(MeshRenderer mesh in carRenderers)
        {
            foreach (Material mat in mesh.materials)
            {
                mat.SetColor("_BaseColor", newColor);
                mat.SetColor("_Color", newColor);
            }
        }
    }
    void CalculateOverlappingGrids()
    {
        float radians = rotationDegrees * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, rotationDegrees, 0));

        int x = scale.x;
        int z = scale.y;

        Vector3 P1 = new Vector3(-(int)x / 2, 0, -(int)z / 2);
        Vector3 P2 = new Vector3((int)x / 2, 0, (int)z / 2);

        Vector3 P1_rotated = rotationMatrix.MultiplyPoint3x4(P1) + spawnPoint;
        Vector3 P2_rotated = rotationMatrix.MultiplyPoint3x4(P2) + spawnPoint;


        FindGridsBetween(P1_rotated, P2_rotated);
    }

    void FindGridsBetween(Vector3 P1, Vector3 P2)
    {
        int x1 = Mathf.RoundToInt(P1.x);
        int z1 = Mathf.RoundToInt(P1.z);
        int x2 = Mathf.RoundToInt(P2.x);
        int z2 = Mathf.RoundToInt(P2.z);

        int dx = Mathf.Abs(x2 - x1);
        int dz = Mathf.Abs(z2 - z1);
        int sx = (x1 < x2) ? 1 : -1;
        int sz = (z1 < z2) ? 1 : -1;

       

        if (dx > dz)
        {
            int err = dx / 2;
            while (x1 != x2)
            {
                gridHolders.Add(new Vector2Int(x1, z1));

                err -= dz;
                if (err < 0)
                {
                    z1 += sz;
                    err += dx;
                }
                x1 += sx;
            }
        }
        else
        {
            int err = dz / 2;
            while (z1 != z2)
            {
                gridHolders.Add(new Vector2Int(x1, z1));

                err -= dx;
                if (err < 0)
                {
                    x1 += sx;
                    err += dz;
                }
                z1 += sz;
            }
        }

        gridHolders.Add(new Vector2Int(x2, z2));

        if (directionType == DirectionType.Up || directionType == DirectionType.Down || directionType == DirectionType.Left || directionType == DirectionType.Right)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0) continue;
                Vector2Int vector2Int = new Vector2Int(gridHolders[0].x + i, gridHolders[0].y);

                if (levelController.GridDict.ContainsKey(vector2Int))
                {
                    //Debug.Log(1);
                    GridRoad gridRoad = (GridRoad)levelController.GridDict[vector2Int];
                    gridRoad.AddCrossCar(this);
                    gridCrossHolders.Add(vector2Int);
                }

                vector2Int = new Vector2Int(gridHolders[0].x, gridHolders[0].y + i);
                if (levelController.GridDict.ContainsKey(vector2Int))
                {
                    GridRoad gridRoad = (GridRoad)levelController.GridDict[vector2Int];
                    gridRoad.AddCrossCar(this);
                    gridCrossHolders.Add(vector2Int);
                }

                vector2Int = new Vector2Int(gridHolders[gridHolders.Count - 1].x + i, gridHolders[gridHolders.Count - 1].y);

                if (levelController.GridDict.ContainsKey(vector2Int))
                {
                    GridRoad gridRoad = (GridRoad)levelController.GridDict[vector2Int];
                    gridRoad.AddCrossCar(this);
                    gridCrossHolders.Add(vector2Int);
                }

                vector2Int = new Vector2Int(gridHolders[gridHolders.Count - 1].x, gridHolders[gridHolders.Count - 1].y + i);
                if (levelController.GridDict.ContainsKey(vector2Int))
                {
                    GridRoad gridRoad = (GridRoad)levelController.GridDict[vector2Int];
                    gridRoad.AddCrossCar(this);
                    gridCrossHolders.Add(vector2Int);
                }
            }
        }

        foreach (var grid in gridHolders)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0) continue;
                Vector2Int vector2Int = new Vector2Int(grid.x + i, grid.y);
                //Debug.Log(vector2Int);
                if (levelController.GridDict.ContainsKey(vector2Int))
                {
                   
                }

                vector2Int = new Vector2Int(grid.x, grid.y + i);
                //Debug.Log(vector2Int);
                if (levelController.GridDict.ContainsKey(vector2Int))
                {
               
                }

            }
            if (levelController.GridDict.ContainsKey(grid))
            {
                GridRoad gridRoad = (GridRoad)levelController.GridDict[grid];
                gridRoad.SetCar(this);

                //Debug.Log(gridRoad.gameObject.name);
                //levelController.RoadDict[grid].AddCrossCar(this);
            }
            //Debug.Log(grid);
        }
    }

    public void Clicked(Action action)
    {
        if (isMoving) return;

        if(stat != CarStat.OnRoad) return;
        //Debug.Log("Clicked");
        clickedAction = action;

        Vector2Int moveDirection = new Vector2Int();
        switch (directionType)
        {
            case DirectionType.Up:
                moveDirection = new Vector2Int(0, 1);
                break;
            case DirectionType.Down:
                moveDirection = new Vector2Int(0, -1);
                break;
            case DirectionType.Left:
                moveDirection = new Vector2Int(-1, 0);
                break;
            case DirectionType.Right:
                moveDirection = new Vector2Int(1, 0);
                break;
            case DirectionType.UpLeft:
                moveDirection = new Vector2Int(-1, 1);
                break;
            case DirectionType.DownLeft:
                moveDirection = new Vector2Int(-1, -1);
                break;
            case DirectionType.UpRight:
                moveDirection = new Vector2Int(1, 1);
                break;
            case DirectionType.DownRight:
                moveDirection = new Vector2Int(1, -1);
                break;
        }

        movePoints.Clear();
        isMoving = true;
        Vector2Int spawnPoint2Int = new Vector2Int(spawnPoint.x, spawnPoint.z);

        if (levelController.CheckRoad(spawnPoint2Int, moveDirection, this))
        {
            ChangeStat(CarStat.MovingToExitRoad);

            moveCoroutine = StartCoroutine(Move());
            foreach (Vector2Int grid in gridHolders)
            {
                GridRoad gridRoad = (GridRoad)levelController.GridDict[grid];
                    
                gridRoad.RemoveCar(this);
            }
            foreach (Vector2Int grid in gridCrossHolders)
            {
                GridRoad gridRoad = (GridRoad)levelController.GridDict[grid];

                gridRoad.RemoveCrossCar(this);
            }

            //levelController.CurrentCar = this;
        }
        else
        {
            moveCoroutine = StartCoroutine(Move());
        }
    }

    public void AddToMovePoints(Vector2Int vector2Int)
    {
        movePoints.Add(new Vector3(vector2Int.x, transform.position.y, vector2Int.y));
    }

    public void AddRangeToMovePoints(List<Vector3> vector3s)
    {
        movePoints.AddRange(vector3s);
    }

    IEnumerator Move(bool activateAction = false, bool isMovingToExitRoad = false)
    {

        for (int i = 1; i < movePoints.Count; i++)
        {
            Vector3 currentPoint = movePoints[i - 1];
            Vector3 nextPoint = movePoints[i];

            Vector3 direction = nextPoint - currentPoint;

            StartCoroutine(SmoothRotateTowardsDirection(direction));


            Vector3 targetPosition = new Vector3(nextPoint.x, nextPoint.y, nextPoint.z);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        if (activateAction && isMovingToExitRoad)
        {
            clickedAction.Invoke();
        }

        if (isMovingToExitRoad)
        {
            ChangeStat(CarStat.OnExitRoad);
        } 
       
        isMoving = false;
        movePoints.Clear();
    }

    IEnumerator MoveBack()
    {
  
        movePoints.Reverse();
        //movePoints.RemoveAt(0);

        yield return null;


        for (int i = 1; i < movePoints.Count; i++)
        {
            //Debug.Log(movePoints[i]);
            Vector3 point = movePoints[i];
            Vector3 targetPosition = new Vector3(point.x, point.y, point.z);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 2 * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        while (Vector3.Distance(transform.position, spawnPoint) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, spawnPoint, moveSpeed * 2 * Time.deltaTime);
            yield return null;
        }

        movePoints.Clear();
        clickedAction.Invoke();
        isMoving = false;
    }

    IEnumerator MoveOutOfMap()
    {
        yield return new WaitForSeconds(1.5f);

        transform.localScale = originalScale;
        SetActiveNeedToHideGameObjectd(true);
        movePoints.Clear();
        isMoving = true;
        movePoints.Add(gridExitStopRoad.GridExitEnterRoad.GetTransformPosition());
        GridMainRoad gridMainRoad = gridExitStopRoad.GridExitEnterRoad.MainRoad;
        movePoints.Add(gridMainRoad.GetTransformPosition());
        movePoints.AddRange(levelController.FindShortestPathToExitMainRoad(gridMainRoad));
        GetComponent<Collider>().enabled = false;

        Vector3 targetPosition = new Vector3(movePoints[0].x, movePoints[0].y, movePoints[0].z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 3 * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        for (int i = 1; i < movePoints.Count; i++)
        {
            //Debug.Log(i);
            Vector3 currentPoint = movePoints[i - 1];
            Vector3 nextPoint = movePoints[i];

            Vector3 direction = nextPoint - currentPoint;

            StartCoroutine(SmoothRotateTowardsDirection(direction, 3));


            targetPosition = new Vector3(nextPoint.x, 0, nextPoint.z);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * 3 * Time.deltaTime);
                yield return null;
            }

            transform.position = targetPosition;
        }

        ChangeStat(CarStat.OnOutOfMap);

        gridExitStopRoad.RemoveCar(this);
        isMoving = false;
        InputManager.Instance.SetCanClickOnCar(true);
        levelController.CheckLevelCompletedCondition();
        gameObject.SetActive(false);
    }
    public IEnumerator PlayShakingAnimation()
    {
        carShakingAnimation.Play();

        yield return new WaitForSeconds(3);

        carShakingAnimation.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        Car carScript = other.GetComponent<Car>();

        if (carScript != null && isMoving)
        {
            //Debug.Log("Touched Car!");
            StartCoroutine(carScript.PlayShakingAnimation());
            
            StopCoroutine(moveCoroutine);
            StartCoroutine(MoveBack());
            moveCoroutine = null;
        }

    }

    IEnumerator SmoothRotateTowardsDirection(Vector3 direction, int time = 1)
    {
        direction.Normalize();

        float targetRotationY = 0f;

        targetRotationY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, targetRotationY, 0f);

        float rotationSpeed = 10f * time; 

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void RoadOptional<T>(List<T> roads) where T : Grid
    {
        StartCoroutine(WaitUntilReachLastMovePointToShowOption(roads));
    }

    IEnumerator WaitUntilReachLastMovePointToShowOption<T>(List<T> roads) where T : Grid
    {
        yield return new WaitUntil(() => !isMoving);

        isMoving = true;
        if (roads[0] is GridMainRoad)
        {
            isMoving = false;
            levelController.ShowArrowExitArea(true);
        }
        else if (roads[0] is GridExitEnterRoad)
        {
            List<GridExitEnterRoad> tmp = new List<GridExitEnterRoad>();
            foreach (var road in roads)
            {
                GridExitEnterRoad r = road as GridExitEnterRoad;
                if (!r.ExitStopRoad.IsHadCar() && r.ExitStopRoad.IsOpen)
                {
                    tmp.Add(r);    
                }
            }
            if (tmp.Count == 1)
            {
                GridExitEnterRoad gridExitEnterRoad = tmp[0];
                //movePoints.Clear();
                movePoints.Add(gridExitEnterRoad.GetTransformPosition());
                movePoints.Add(gridExitEnterRoad.ExitStopRoad.GetTransformPosition());
                gridExitEnterRoad.ExitStopRoad.SetCar(this);
                gridExitStopRoad = gridExitEnterRoad.ExitStopRoad;
                gridExitEnterRoad.ExitArea.CarInExitStops.Add(this);
                //Debug.Log(1);
                StartCoroutine(Move(true, true));
            }
            else
            {
                //Hien thi bang chon duong di
            }
        }
    }

    public void ChoicedExitArea(GridMainRoad gridMainRoad, ExitArea exitArea)
    {
        var result = levelController.FindShortestPathToExitEnterRoad(gridMainRoad, exitArea);
        //Debug.Log(result.Item2.Count);
        movePoints.Clear();
        AddRangeToMovePoints(result.Item1);
        StartCoroutine(Move());
        RoadOptional(result.Item2);
    }

    public void ChangeStat(CarStat stat)
    {
        this.stat = stat;

        switch (stat)
        {
            case CarStat.OnRoad:

                break;
            case CarStat.MovingToExitRoad:

                break;
            case CarStat.OnExitRoad:
                OnEnterOnExitRoad();
                break;
            case CarStat.MovingOutOfMap:
                OnMovingOutOfMap();
                break;
            case CarStat.OnOutOfMap:

                break;
        }
    }

    void OnEnterOnExitRoad()
    {
        SetActiveNeedToHideGameObjectd(false);
        transform.localScale = Vector3.one;

        GetPassenger();
      
    }

    void SetActiveNeedToHideGameObjectd(bool b)
    {
        foreach (GameObject gameObject in needToHideObjects)
        {
            gameObject.SetActive(b);
        }
    }

    void GetPassenger()
    {   
        if(IsFullOfSeat() || stat != CarStat.OnExitRoad ) return;
        //Debug.Log("Get Passenger " + colorType);
        List<Passenger> passengers = gridExitStopRoad.GetPassenger(this);
        
        bool check = false;
        foreach (Passenger passenger in passengers)
        {

            for(int i = 0; i < seats.Count; i++)
            {
                Seat seat = seats[i];
                if (!seat.hadTakenSeat)
                {
                    this.passengers.Add(passenger);
                    passenger.MovePoints.Add(seat.transform.position);
                    passenger.transform.parent = seat.transform;
                    passenger.Move();
                    passenger.ChangeStat(PassengerStat.OnPickedUp);
                    seat.hadTakenSeat = true;
                    check = true;
                    if (i == seats.Count - 1)
                    {
                        ChangeStat(CarStat.MovingOutOfMap);
                        
                        gridExitStopRoad.GridExitEnterRoad.ExitArea.CarInExitStops.Remove(this);
                        return;
                        //return true;
                    }

                    break;
                }
            }
         
        }

        if (!check)
        {

        }
    }

    void OnMovingOutOfMap()
    {
        InputManager.Instance.SetCanClickOnCar(false);
        foreach (Passenger passenger in passengers)
        {
            passenger.ExitArea.PassengerList.Remove(passenger);
        }
        StartCoroutine(MoveOutOfMap()); 
    }

    bool isFull = false;
    public bool IsFullOfSeat()
    {
        foreach(Seat seat in seats)
        {
            if (!seat.hadTakenSeat)
            {
                isFull = false ;
                return false;
            }
        }

        isFull = true;
        return true;
    }
}

public enum DirectionType
{
    Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
}

public enum CarStat
{
    OnRoad, MovingToExitRoad, OnExitRoad, MovingOutOfMap, OnOutOfMap 
}

public enum ColorType
{
    Red, Green, Blue
}