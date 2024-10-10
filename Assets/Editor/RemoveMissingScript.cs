using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RemoveMissingScript : MonoBehaviour
{
    [MenuItem("Tools/Remove Missing Scripts in Scene")]
    static void RemoveAllMissingScripts()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        int missingScriptCount = 0;

        foreach (GameObject go in allObjects)
        {
            if (go.hideFlags == HideFlags.None && !EditorUtility.IsPersistent(go))
            {
                int countBefore = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

                if (countBefore > 0)
                {
                    Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    int countAfter = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

                    missingScriptCount += (countBefore - countAfter);
                    EditorUtility.SetDirty(go);
                }
            }
        }

        Debug.Log($"Removed {missingScriptCount} missing scripts from all GameObjects in the scene.");
    }
}
