
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDesigner))]
public class LevelDesignerInput : Editor 
{
    private LayerMask targetLayerMask = 1 << 6 | 1 << 7;
    private LayerMask targetLayerMaskCar = 1 << 8 | 1 << 10;
    private void OnSceneGUI()
    {
        LevelDesigner levelDesignManager = (LevelDesigner)target;
        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            switch (e.keyCode)
            {
                case KeyCode.Alpha1:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.DeleteGrid(hit.point);
                    }

                    break;
                case KeyCode.Alpha2:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.SpawnMainRoad(hit.point);
                    }
                    break;
                case KeyCode.Alpha3:
                    
                    break;
                case KeyCode.Alpha4:

                    break;
                case KeyCode.Alpha5:
                   
                    break;
                case KeyCode.Alpha6:
                   
                    break;
            }

            e.Use();
        }

    }
}
