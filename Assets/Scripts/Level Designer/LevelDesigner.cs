using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelDesigner : MonoBehaviour
{
    [SerializeField] LevelDesignerManager levelDesignerManager;
    public Dictionary<Vector2Int, Grid> GridDict => levelDesignerManager.GridDict;
    public Dictionary<Vector2Int, Car> CarDict => levelDesignerManager.CarDict;

    [SerializeField] GameObject mainRoadPref;
    [SerializeField] Transform mainRoadParent;

    [SerializeField] List<GameObject> exitAreaPrefs;
    [SerializeField] Transform exitAreaParent;

    [SerializeField] GameObject car4SeatPref;
    [SerializeField] GameObject car6SeatPref;
    [SerializeField] GameObject car10SeatPref;
    [SerializeField] Transform carParent;

    [SerializeField] GameObject roadPref;
    [SerializeField] Transform roadParent;

    [SerializeField] GameObject borderRoadPref;
    [SerializeField] Transform borderRoadParent;
    public void DeleteGrid(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!CheckGrid(grid3D))
        {
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);

            if(GridDict[grid] is GridExitArea)
            {
                Debug.Log("Deleted Exit Area !");
                RemoveSameValueGrid(GridDict[grid]);
            }
            else
            {
                Debug.Log("Deleted Road !");
                DestroyImmediate(GridDict[grid].gameObject);
                GridDict.Remove(grid);
            }
            
            //Debug.Log("Grid Dict Count : " + gridDict.Count);
            return;
        }
    }

    public void DeleteCar(Vector3 vector3)
    {

        Vector3Int grid3D = GetGrid3dPosition(vector3);
        Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
        if (CarDict.ContainsKey(grid))
        {
            Car c = CarDict[grid];
            var keysToRemove = CarDict
                      .Where(kvp => kvp.Value == c)
                      .Select(kvp => kvp.Key)
                      .ToList();

            foreach (var key in keysToRemove)
            {
                CarDict.Remove(key);
            }

            DestroyImmediate(c.gameObject);
        }
    }

    public void SpawnMainRoad(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!CheckGrid(grid3D))
        {
            Debug.Log("Grid has Existed !!");
            
            return;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(grid3D.x, 0.1f, grid3D.z);
            GameObject newObj = PrefabUtility.InstantiatePrefab(mainRoadPref) as GameObject;
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = mainRoadParent;

            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            GridDict.Add(grid, newObj.GetComponent<GridMainRoad>());
            //Debug.Log("Grid Dict Count : " + gridDict.Count);
        }
    }

    public void SpawnExitArea(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
 
        if (!CheckGrid(grid3D))
        {
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            if (GridDict[grid] is GridExitArea)
            {
                //Xoá cũ và spawn mới
                GridExitArea gridExitArea = (GridExitArea)GridDict[grid];
                int index = gridExitArea.IndexInLevelDesigner;
                index++;
                if(index >= exitAreaPrefs.Count)
                {
                    index = 0;
                }

                RemoveSameValueGrid(GridDict[grid]);
                SpawnExitArea(grid3D, index);
            }
            else
            {
                Debug.Log("Grid has Existed !!");

                return;
            }
        }
        else
        {
            SpawnExitArea(grid3D, 0);
        }
    }

    public void SpawnExitArea(Vector3Int grid3D, int index)
    {
        Vector3 spawnPosition = new Vector3(grid3D.x, 0.1f, grid3D.z);
    
        GridExitArea gridExitArea = exitAreaPrefs[index].GetComponent<GridExitArea>();
       
        Vector2Int area = gridExitArea.ExitArea.ExitAreaSize;

        int halfX = area.x / 2;
        List<Vector2Int> currentGirdHolderList = new List<Vector2Int>();

        if (gridExitArea.ExitArea.ExitAreaType == ExitAreaType.Horizontal)
        {
            for (int i = -halfX; i < halfX; i++)
            {
                for (int j = 0; j < area.y; j++)
                {
                    Vector3 location = new Vector3(i, -0f, j);
                    Vector2Int _grid = new Vector2Int(i, j);

                    Vector2Int vector2Int = _grid + new Vector2Int(grid3D.x, grid3D.z);
                    if (GridDict.ContainsKey(vector2Int))
                    {
                        Debug.Log("Exit Area has Existed !!");
                        return;
                    }
                    currentGirdHolderList.Add(vector2Int);
                }
            }
        }
        else
        {
            for (int i = -halfX; i < halfX; i++)
            {
                for (int j = 0; j < area.y; j++)
                {
                    Vector3 location = new Vector3(j, -0f, i);
                    Vector2Int _grid = new Vector2Int(j, i);

                    Vector2Int vector2Int = _grid + new Vector2Int(grid3D.x, grid3D.z);
                    if (GridDict.ContainsKey(vector2Int))
                    {
                        Debug.Log("Exit Area has Existed !!");
                        return;
                    }
                    currentGirdHolderList.Add(vector2Int);
                }
            }
        }
        GameObject newObj = PrefabUtility.InstantiatePrefab(exitAreaPrefs[index]) as GameObject;
        newObj.transform.position = spawnPosition;
        newObj.transform.parent = exitAreaParent;
        newObj.GetComponent<GridExitArea>().IndexInLevelDesigner = index;

        foreach (Vector2Int v in currentGirdHolderList)
        {
            GridDict.Add(v, newObj.GetComponent<GridExitArea>());
        }
    }

    public void Spawn4SeatCar(Vector3 vector3)
    {
        SpawnCar(vector3, car4SeatPref);
    }

    public void Spawn6SeatCar(Vector3 vector3)
    {
        SpawnCar(vector3, car6SeatPref);
    }

    public void Spawn10SeatCar(Vector3 vector3)
    {
        SpawnCar(vector3, car10SeatPref);
    }

    public void SpawnCar(Vector3 vector3, GameObject carPref)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (CheckGrid(grid3D))
        {
            Debug.Log("Don't have Road Grid !!");

            return;
        }
        else
        {
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            if (GridDict[grid] is GridRoad)
            {
                Vector3 spawnPosition = new Vector3(grid3D.x, 0.2f, grid3D.z);
            
                Car car = carPref.GetComponent<Car>();
                Vector2Int scale = car.Scale;
                float rota = GetRotation(DirectionType.Up);
                List<Vector2Int> carHolder = CalculateOverlappingGrids(rota, scale, grid3D, DirectionType.Up);

                if (CheckCar(carHolder))
                {
                    Debug.Log("Spawn new Car !!");

                    SpawnCar(spawnPosition, carPref, DirectionType.Up, ColorType.Red, carHolder);
                    
                }
                else
                {
                    if (CarDict.ContainsKey(grid))
                    {
                        Debug.Log("Change Car !!");
                        Car c = CarDict[grid];


                        var keysToRemove = CarDict
                            .Where(kvp => kvp.Value == c)
                            .Select(kvp => kvp.Key)
                            .ToList();

                        foreach (var key in keysToRemove)
                        {
                            CarDict.Remove(key);
                        }

                        DirectionType directionType = GetNextDirection(c.DirectionType);
                        Vector3 cTransform = c.transform.position;
                        ColorType colorType = c.ColorType;
                        DestroyImmediate(c.gameObject);

                        SpawnCar(cTransform, carPref, directionType, colorType);
                    }
                    else
                    {
                        Debug.Log("Can't spawn new Car here !");
                    }
                }
            }
            else
            {
                Debug.Log("Need Road Grid to spawn !!");

                return;
            }
        }
    }

    public void SpawnCar(Vector3 spawnPosition, GameObject carPref, DirectionType directionType, ColorType colorType, List<Vector2Int> carHolder = null)
    {
        GameObject newObj = PrefabUtility.InstantiatePrefab(carPref) as GameObject;
        newObj.transform.position = spawnPosition;
        newObj.transform.parent = carParent;

        Car c = newObj.GetComponent<Car>();
        c.DirectionType = directionType;
        c.ColorType = colorType;

        if (carHolder == null)
        {
            Vector2Int scale = c.Scale;
            float rota = GetRotation(directionType);
            carHolder = CalculateOverlappingGrids(rota, scale, new Vector3Int((int)spawnPosition.x, 0, (int)spawnPosition.z), directionType);

        }
        foreach (Vector2Int carHolderTmp in carHolder)
        {
            CarDict.Add(carHolderTmp, c);
        }
        SetColorInSceneGUI(GetColor(c.ColorType), c.CarRenderers);
    }

    public void ChangeCarColor(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
        if (CarDict.ContainsKey(grid))
        {
            Car c = CarDict[grid];
            ColorType color = GetNextColor(c.ColorType);

            c.ColorType = color;
            SetColorInSceneGUI(GetColor(c.ColorType), c.CarRenderers);
        }
    }

    public void SpawnRoad(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!CheckGrid(grid3D))
        {
            Debug.Log("Grid has Existed !!");

            return;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(grid3D.x, 0.1f, grid3D.z);
            GameObject newObj = PrefabUtility.InstantiatePrefab(roadPref) as GameObject;
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = roadParent;

            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            GridDict.Add(grid, newObj.GetComponent<GridRoad>());
        }
    }

    public void SpawnBorderRoad(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!CheckGrid(grid3D))
        {
            Debug.Log("Grid has Existed !!");

            return;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(grid3D.x, 0.1f, grid3D.z);
            GameObject newObj = PrefabUtility.InstantiatePrefab(borderRoadPref) as GameObject;
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = borderRoadParent;


            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            GridDict.Add(grid, newObj.GetComponent<GridBorderRoad>());
        }
    }

    #region checkCondition
    public bool CheckCar(List<Vector2Int> carHolder)
    {
        foreach (Vector2Int car in carHolder)
        {
            if(CarDict.ContainsKey(car))
            {
                return false;
            }
        } 
        return true;
    }

    public bool CheckGrid(Vector3Int vector3)
    {
        if (!CheckExistedDict(vector3))
        {
            return true;
        }

        return false;
    }

    public bool CheckExistedDict(Vector3Int vector3)
    {
        Vector2Int grid = new Vector2Int(vector3.x, vector3.z);
        if (!GridDict.ContainsKey(grid))
        {
            return false;
        }
        else
        {          
            return true;
        }
    }

    #endregion


    #region helper
    void RemoveSameValueGrid(Grid gridA)
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();

        foreach (var entry in GridDict)
        {
            if (entry.Value == gridA)
            {
                keysToRemove.Add(entry.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            GridDict.Remove(key);         
        }
        DestroyImmediate(gridA.gameObject);
    }

    public Vector3Int GetGrid3dPosition(Vector3 vector3)
    {
        Vector3Int gridPosition = Vector3Int.RoundToInt(vector3);

        return gridPosition;
    }

    List<Vector2Int> CalculateOverlappingGrids(float rotationDegrees, Vector2Int scale, Vector3Int spawnPoint, DirectionType directionType)
    {
        float radians = rotationDegrees * Mathf.Deg2Rad;

        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, rotationDegrees, 0));

        int x = scale.x;
        int z = scale.y;

        Vector3 P1 = new Vector3(-(int)x / 2, 0, -(int)z / 2);
        Vector3 P2 = new Vector3((int)x / 2, 0, (int)z / 2);

        Vector3 P1_rotated = rotationMatrix.MultiplyPoint3x4(P1) + spawnPoint;
        Vector3 P2_rotated = rotationMatrix.MultiplyPoint3x4(P2) + spawnPoint;

        return FindGridsBetween(P1_rotated, P2_rotated, directionType);
    }
    List<Vector2Int> FindGridsBetween(Vector3 P1, Vector3 P2, DirectionType directionType)
    {
        List<Vector2Int> carHolder = new List<Vector2Int>();

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
                carHolder.Add(new Vector2Int(x1, z1));

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
                carHolder.Add(new Vector2Int(x1, z1));

                err -= dx;
                if (err < 0)
                {
                    x1 += sx;
                    err += dz;
                }
                z1 += sz;
            }
        }

        carHolder.Add(new Vector2Int(x2, z2));
        foreach (var car in carHolder)
        {
        }
        return carHolder;
    }

    float GetRotation(DirectionType directionType)
    {
        float rotationDegrees = 0;
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

       return rotationDegrees;
    }

    public DirectionType GetNextDirection(DirectionType currentDirection)
    {
        switch (currentDirection)
        {
            case DirectionType.Up:
                return DirectionType.Down; 
            case DirectionType.Down:
                return DirectionType.Left; 
            case DirectionType.Left:
                return DirectionType.Right; 
            case DirectionType.Right:
                return DirectionType.UpLeft; 
            case DirectionType.UpLeft:
                return DirectionType.UpRight; 
            case DirectionType.UpRight:
                return DirectionType.DownLeft; 
            case DirectionType.DownLeft:
                return DirectionType.DownRight; 
            case DirectionType.DownRight:
                return DirectionType.Up; 
            default:
                return currentDirection; 
        }
    }
    public ColorType GetNextColor(ColorType currentColor)
    {
        switch (currentColor)
        {
            case ColorType.Red:
                return ColorType.Green;  
            case ColorType.Green:
                return ColorType.Blue;   
            case ColorType.Blue:
                return ColorType.Yellow; 
            case ColorType.Yellow:
                return ColorType.Purple;
            case ColorType.Purple:
                return ColorType.Red;   
            default:
                return currentColor;     
        }
    }

    void SetColorInSceneGUI(Color newColor, List<MeshRenderer> carRenderers)
    {
        foreach (MeshRenderer mesh in carRenderers)
        {
            Material[] sharedMaterials = mesh.sharedMaterials;
            Material[] newMaterials = new Material[sharedMaterials.Length];

            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                newMaterials[i] = new Material(sharedMaterials[i]);
                newMaterials[i].SetColor("_BaseColor", newColor);
                newMaterials[i].SetColor("_Color", newColor);
            }
            mesh.materials = newMaterials;
        }
    }
    public Color GetColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Red:
                return Color.red;
           
            case ColorType.Green:
                return Color.green;
                
            case ColorType.Blue:
                return Color.blue;
                
            case ColorType.Yellow:
                return Color.yellow;
                
            case ColorType.Purple:
                return new Color(0.5f, 0f, 0.5f);
                
        }

        return Color.white;
    }
    #endregion
}
