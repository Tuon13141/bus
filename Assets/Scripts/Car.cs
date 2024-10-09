using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Car : MonoBehaviour
{
    public int scaleX; 
    public int scaleZ; 
    public DirectionType directionType;
    private float rotationDegrees; 
    public Vector3Int spawnPoint;

    void Start()
    {
        transform.position = spawnPoint;
        GetRotation();
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
    void CalculateOverlappingGrids()
    {
        float radians = rotationDegrees * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, rotationDegrees, 0));

        int x = scaleX;
        int z = scaleZ;

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

        List<Vector2Int> grids = new List<Vector2Int>();

        if (dx > dz)
        {
            int err = dx / 2;
            while (x1 != x2)
            {
                grids.Add(new Vector2Int(x1, z1));

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
                grids.Add(new Vector2Int(x1, z1));

                err -= dx;
                if (err < 0)
                {
                    x1 += sx;
                    err += dz;
                }
                z1 += sz;
            }
        }

        grids.Add(new Vector2Int(x2, z2));

        foreach (var grid in grids)
        {
            for(int i = -1; i <=1; i++)
            {
                if (i == 0) continue;
                Vector2Int vector2Int = new Vector2Int(grid.x + i, grid.y);
                //Debug.Log(vector2Int);
                if (LevelController.Instance.RoadDict.ContainsKey(vector2Int))
                {
                    //Debug.Log(1);
                    LevelController.Instance.RoadDict[vector2Int].SetCar(this);
                }

                vector2Int = new Vector2Int(grid.x, grid.y + i);
                //Debug.Log(vector2Int);
                if (LevelController.Instance.RoadDict.ContainsKey(vector2Int))
                {
                    //Debug.Log(1);
                    LevelController.Instance.RoadDict[vector2Int].SetCar(this);
                }
            }
            if (LevelController.Instance.RoadDict.ContainsKey(grid))
            {
                LevelController.Instance.RoadDict[grid].SetCar(this);
            }
            //Debug.Log(grid);
        }
    }

    public void Clicked()
    {
        Debug.Log("Clicked");
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


        LevelController.Instance.CheckRoad(new Vector2Int(spawnPoint.x, spawnPoint.z), moveDirection, this);
    }
}

public enum DirectionType
{
    Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
}
