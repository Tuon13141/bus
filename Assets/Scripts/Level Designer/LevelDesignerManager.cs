using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDesignerManager : SerializedMonoBehaviour
{
    [SerializeField] LevelDesigner levelDesigner;

    [FoldoutGroup("Data")]
    public Dictionary<Vector2Int, Grid> GridDict = new Dictionary<Vector2Int, Grid>();


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
        foreach (Grid grid in GridDict.Values)
        {
            DestroyImmediate(grid.gameObject);
        }
        GridDict.Clear();
        levelDesigner.UnusableGrid.Clear();

        Debug.Log("Level Has been cleared !");
    }


    [PropertySpace(10)]
    [PropertyOrder(2)]
    [Button, GUIColor(0.4f, 0.8f, 1)]
    public void LoadLevel()
    {
        string localPath = "Assets/Resources/Levels/Level_" + LevelNum + ".prefab";

        if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject)))
        {
            DestroyImmediate(currentLevelObject);
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
