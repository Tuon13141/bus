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
    public GameObject LevelPref;

    GameObject currentLevelObject;

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
            if(GridDict[grid] != null)
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

       
        Debug.Log("Level Has been cleared !");
    }

    [PropertySpace(10)]    
    [Button, GUIColor(1, 0.5f, 0)]
    public void NewLevel()
    {
        ClearLevel();
        GameObject newGameObj = PrefabUtility.InstantiatePrefab(LevelPref) as GameObject;
         
        newGameObj.transform.position = Vector3.zero;
        currentLevelObject = newGameObj;

        Debug.Log("Created new Level !");
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
            return;
        }
        else
        {
            Debug.Log("Level Doesn't exist !");
        }
    }

    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(0.4f, 0.8f, 1)]
    public void SaveLevel()
    {
        //localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        // PrefabUtility.SaveAsPrefabAsset(Level.gameObject, localPath, out prefabSuccess);
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
            return;
        }
        else
        {
            Debug.Log("Level Doesn't exist !");
        }
    }

#endif
}
