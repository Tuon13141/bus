using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{
    Dictionary<Vector2Int, Grid> gridDict = new Dictionary<Vector2Int, Grid>();


    [SerializeField] GameObject mainRoadPref;
    [SerializeField] Transform mainRoadParent;

    public void DeleteGrid(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (CheckExistedDict(grid3D))
        {
            Debug.Log("Delete Grid");
            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            DestroyImmediate(gridDict[grid].gameObject);
            gridDict.Remove(grid);
            Debug.Log("Grid Dict Count : " + gridDict.Count);
            return;
        }
    }

    public void SpawnMainRoad(Vector3 vector3)
    {
        Vector3Int grid3D = GetGrid3dPosition(vector3);
        if (CheckExistedDict(grid3D))
        {
            Debug.Log("Grid has Existed !!");
            
            return;
        }
        else
        {
            Vector3 spawnPosition = new Vector3(grid3D.x, -0.3f, grid3D.z);
            GameObject newObj = PrefabUtility.InstantiatePrefab(mainRoadPref) as GameObject;
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = mainRoadParent;

            Vector2Int grid = new Vector2Int(grid3D.x, grid3D.z);
            gridDict.Add(grid, newObj.GetComponent<GridMainRoad>());

            Debug.Log("Grid Dict Count : " + gridDict.Count);
        }
    }

    public Vector3Int GetGrid3dPosition(Vector3 vector3)
    {
        Vector3Int gridPosition = Vector3Int.RoundToInt(vector3);

        return gridPosition;
    }

    public bool CheckExistedDict(Vector3Int vector3)
    {
        Vector2Int grid = new Vector2Int(vector3.x, vector3.z);
        if (!gridDict.ContainsKey(grid))
        {
            return false;
        }
        else
        {          
            return true;
        }
    }
}
