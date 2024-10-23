
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDesigner), true)]
public class LevelDesignerInput : Editor 
{
    private LayerMask targetLayerMask = 1 << 6;
    private LayerMask targetLayerMaskMainRoad = 1 << 8 | 1 << 10;
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
                case KeyCode.Alpha0:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.DeleteCar(hit.point);
                    }

                    break;
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
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.SpawnExitArea(hit.point);
                    }
                    break;
                case KeyCode.Alpha4:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.SpawnRoad(hit.point);
                    }
                    break;
                case KeyCode.Alpha5:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.SpawnBorderRoad(hit.point);
                    }
                    break;
                case KeyCode.Alpha6:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.Spawn4SeatCar(hit.point);
                    }
                    break;
                case KeyCode.Alpha7:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.Spawn6SeatCar(hit.point);
                    }
                    break;
                case KeyCode.Alpha8:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.Spawn10SeatCar(hit.point);
                    }
                    break;
                case KeyCode.Alpha9:
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayerMask))
                    {
                        levelDesignManager.ChangeCarColor(hit.point);
                    }
                    break;
            }

            e.Use();
        }

    }
}
