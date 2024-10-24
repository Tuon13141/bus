using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelDesignerManager : SerializedMonoBehaviour
{
    [SerializeField] LevelDesigner levelDesigner;

    [FoldoutGroup("Data")]
    public Dictionary<Vector2Int, Grid> GridDict = new Dictionary<Vector2Int, Grid>();
    [FoldoutGroup("Data")]
    public Dictionary<Vector2Int, Car> CarDict = new Dictionary<Vector2Int, Car>();
    [FoldoutGroup("Data")]
    public List<GameObject> ExitAreaObjects = new List<GameObject>();
    [FoldoutGroup("Data")]
    public GameObject LevelPref;

    [SerializeField] GameObject currentLevelObject;

#if UNITY_EDITOR

    [PropertySpace(10)]
    [PropertyOrder(0)]
    [LabelText("Level")]
    [OnValueChanged("ChangeLevel")]
    public int LevelNum = -1;

    public void ChangeLevel()
    {
        //Debug.Log(LevelNum);
    }

    [PropertySpace(10)]
    [PropertyOrder(1)]
    [Button, GUIColor(1, 0.5f, 0)]
    public void ClearLevel()
    {
        if (currentLevelObject != null)
        {
            DestroyImmediate(currentLevelObject);
        }
        foreach (Vector2Int grid in GridDict.Keys)
        {
            if (GridDict[grid] != null)
                DestroyImmediate(GridDict[grid].gameObject);
        }
        foreach (Vector2Int grid in CarDict.Keys)
        {
            if (CarDict[grid] != null)
                DestroyImmediate(CarDict[grid].gameObject);
        }

        RemoveNullValues();
        GridDict.Clear();
        CarDict.Clear();
        ExitAreaObjects.Clear();


        Debug.Log("Level Has been cleared !");
    }

    void RemoveNullValues()
    {
        var keysToRemove = GridDict
            .Where(entry => entry.Value == null)
            .Select(entry => entry.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            GridDict.Remove(key);
        }

        var carsToRemove = CarDict
           .Where(entry => entry.Value == null)
           .Select(entry => entry.Key)
           .ToList();

        foreach (var key in carsToRemove)
        {
            CarDict.Remove(key);
        }
    }
    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(0.4f, 0.8f, 1)]
    public void LoadLevel()
    {
        string localPath = "Assets/Resources/Levels/Level_" + LevelNum + ".prefab";

        if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
        {
            ClearLevel();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
            currentLevelObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            LevelRenderer levelRenderer = currentLevelObject.GetComponent<LevelRenderer>();

            SpawnRoads(levelRenderer.borderRoadGridPositionList, levelDesigner.BorderRoadPref, levelDesigner.borderRoadParent);
            SpawnRoads(levelRenderer.roadGridPositionList, levelDesigner.RoadPref, levelDesigner.roadParent);
            for (int k = 0; k < levelRenderer.ExitAreas.Count; k++)
            {
                ExitArea exitArea = levelRenderer.ExitAreas[k];
                GameObject exitAreaObject = exitArea.gameObject;
                Vector3Int grid3D = levelDesigner.GetGrid3dPosition(exitAreaObject.transform.position);
                Vector2Int area = exitArea.ExitAreaSize;

                int halfX = area.x / 2;

                if (exitArea.ExitAreaType == ExitAreaType.Horizontal)
                {
                    for (int i = -halfX; i < halfX; i++)
                    {
                        for (int j = 0; j < area.y; j++)
                        {
                            Vector3 location = new Vector3(i, -0f, j);
                            Vector2Int _grid = new Vector2Int(i, j);

                            Vector2Int vector2Int = _grid + new Vector2Int(grid3D.x, grid3D.z);

                            GridDict.Add(vector2Int, exitAreaObject.GetComponent<GridExitArea>());
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

                            GridDict.Add(vector2Int, exitAreaObject.GetComponent<GridExitArea>());
                        }
                    }
                }

                ExitAreaObjects.Add(exitAreaObject);
            }
            foreach (Car car in levelRenderer.CarList)
            {
                CarDict.Add(new Vector2Int((int)car.transform.position.x, (int)car.transform.position.z), car);
                levelDesigner.SetColorInSceneGUI(levelDesigner.GetColor(car.ColorType), car.CarRenderers);
            }
            return;
        }
        else
        {
            Debug.Log("Level Doesn't exist !");
        }
    }
    public void SpawnRoads(List<Vector2Int> vector2Ints, GameObject pref, Transform parent)
    {
        foreach (Vector2Int vector2Int in vector2Ints)
        {
            Vector3 location = new Vector3(vector2Int.x, 0.1f, vector2Int.y);
            GameObject road = (GameObject)PrefabUtility.InstantiatePrefab(pref);
            road.transform.position = location;
            road.transform.parent = parent;
            if (!GridDict.ContainsKey(vector2Int))
            {
                GridDict.Add(vector2Int, road.GetComponent<Grid>());
            }
            else
            {
                DestroyImmediate(road);
            }
        }
    }


    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(0.4f, 0.8f, 1)]
    public void CreateLevel()
    {
        string localPath = "Assets/Resources/Levels/Level_" + LevelNum + ".prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(localPath) != null)
        {
            Debug.Log("Prefab already exists at: " + localPath + " !");
            return;
        }

        Save(localPath);

    }
    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(0.4f, 0.8f, 1)]
    public void SaveLevel()
    {
        string localPath = "Assets/Resources/Levels/Level_" + LevelNum + ".prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(localPath) == null)
        {
            Debug.Log("Level doesn't exists at: " + localPath + " !");
            return;
        }

        Save(localPath);
        LoadLevel();
    } 

    void Save(string localPath)
    {
        bool hasExitArea = GridDict.Values.Any(grid => grid is GridExitArea);
        if (!hasExitArea || CarDict.Values.Count <= 0 || ExitAreaObjects.Count <= 0)
        {
            Debug.Log("Make sure to put Car and Area Exit in Level !");
            return;
        }
        if (currentLevelObject == null)
        {
            GameObject newGameObj = PrefabUtility.InstantiatePrefab(LevelPref) as GameObject;

            newGameObj.transform.position = Vector3.zero;
            currentLevelObject = newGameObj;

        }

        List<Vector2Int> roadGrids = new List<Vector2Int>();

        List<Vector2Int> borderRoadGrids = new List<Vector2Int>();

        List<GameObject> mainRoadGridObjects = new List<GameObject>();

        List<Car> cars = CarDict
            .Select(kvp => kvp.Value)
            .Distinct()
            .ToList();

        roadGrids = GridDict
            .Where(kvp => kvp.Value is GridRoad)
            .Select(kvp => kvp.Key)
            .ToList();

        borderRoadGrids = GridDict
            .Where(kvp => kvp.Value is GridBorderRoad)
            .Select(kvp => kvp.Key)
            .ToList();

        mainRoadGridObjects = GridDict
            .Where(kvp => kvp.Value is GridMainRoad)
            .Select(kvp => ((GridMainRoad)kvp.Value).gameObject)
            .ToList();
        bool prefabSuccess;

        LevelRenderer levelRenderer = currentLevelObject.GetComponent<LevelRenderer>();
        levelRenderer.roadGridPositionList = roadGrids;
        levelRenderer.borderRoadGridPositionList = borderRoadGrids;
        levelRenderer.AddMainRoadObjects(mainRoadGridObjects);
        levelRenderer.AddExitAreas(ExitAreaObjects);
        levelRenderer.AddCars(cars);

        prefabSuccess = PrefabUtility.SaveAsPrefabAssetAndConnect(currentLevelObject, localPath, InteractionMode.AutomatedAction);


        if (prefabSuccess)
        {
            Debug.Log("Prefab saved successfully at: " + localPath);
        }
        else
        {
            Debug.LogError("Failed to save prefab at: " + localPath);
        }
    }

    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(1, 0.2f, 0)]
    public void DeleteLevel()
    {
        string localPath = "Assets/Resources/Levels/Level_" + LevelNum + ".prefab";

        if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
        {
            AssetDatabase.DeleteAsset(localPath);
            Debug.Log("Deleted Level_" + LevelNum + " !");
            return;
        }
        else
        {
            Debug.Log("Level Doesn't exist !");
        }
    }

#endif
}
