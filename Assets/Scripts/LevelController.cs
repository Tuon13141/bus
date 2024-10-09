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
        Debug.Log("Checking");
        Vector2Int nextGridRoad = startPosition + moveDirection;

        if (roadDict.ContainsKey(nextGridRoad))
        {
            Debug.Log(1);
            if (roadDict[nextGridRoad] is GridMainRoad)
            {
                Debug.Log("Find Main Road");
                return true;
            }
            else if (roadDict[nextGridRoad] is GridBorderRoad)
            {
                Debug.Log("Find Border Road");
                return true;
            }
            else if (roadDict[nextGridRoad] is GridRoad)
            {
                Debug.Log("Find Road");
                if (RoadDict[nextGridRoad].IsHadCar(car))
                {
                    Debug.Log("Find Car");
                    return false;
                }
                return CheckRoad(nextGridRoad, moveDirection, car);
            }
            else
            {
                Debug.Log(3);
            }
        }
        else
        {
            Debug.Log(2);
        }

        return false;
    }
}
