using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    [SerializeField] LevelDesignerManager levelDesignerManager;
    public List<Vector2Int> UnusableGrid { get; set; } = new List<Vector2Int>();
    public Dictionary<Vector2Int, Grid> GridDict => levelDesignerManager.GridDict;


    [SerializeField] GameObject mainRoadPref;
    [SerializeField] Transform mainRoadParent;

    [SerializeField] List<GameObject> exitAreaPrefs;
    public void DeleteGrid(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!Check(grid3D))
        {
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            if (GridDict[grid] is GridMainRoad)
            {
                Debug.Log("Deleted Main Road !");
                UnusableGrid.Remove(grid);
            }
            else if(GridDict[grid] is GridExitArea)
            {
                Debug.Log("Deleted Exit Area !");
                
            }
           
            DestroyImmediate(GridDict[grid].gameObject);
            GridDict.Remove(grid);
            
            //Debug.Log("Grid Dict Count : " + gridDict.Count);
            return;
        }
    }

    public void SpawnMainRoad(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (!Check(grid3D))
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
            UnusableGrid.Add(grid);
            //Debug.Log("Grid Dict Count : " + gridDict.Count);
        }
    }

    public void SpawnExitArea(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
 
        if (!Check(grid3D))
        {
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            if (GridDict[grid] is GridExitArea)
            {
                //Xoá cũ và spawn mới
                RemoveSameValueGrid(GridDict[grid]);
                
            }
            else
            {
                Debug.Log("Grid has Existed !!");

                return;
            }
        }
        else
        {
            Vector3 spawnPosition = new Vector3(grid3D.x, 0.1f, grid3D.z);
            GameObject newObj = PrefabUtility.InstantiatePrefab(mainRoadPref) as GameObject;
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = mainRoadParent;

            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            GridDict.Add(grid, newObj.GetComponent<GridMainRoad>());
            UnusableGrid.Add(grid);
        }
    }

    public Vector3Int GetGrid3dPosition(Vector3 vector3)
    {
        Vector3Int gridPosition = Vector3Int.RoundToInt(vector3);

        return gridPosition;
    }

    public bool Check(Vector3Int vector3)
    {
        if(!CheckExistedDict(vector3) && !CheckUnusableGrid(vector3))
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

    public bool CheckUnusableGrid(Vector3Int vector3)
    {
        Vector2Int grid = new Vector2Int(vector3.x, vector3.z);
        if (!UnusableGrid.Contains(grid))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

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
            DestroyImmediate(GridDict[key].gameObject);
            GridDict.Remove(key);         
        }
    }
    #endregion
}
