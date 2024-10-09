using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    [SerializeField] GameObject mainRoadPref;
    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;

    [SerializeField] Vector2Int levelArea;
  

    [SerializeField] int mainRoadsZ;

    [SerializeField] GameObject roadParent;
    List<GameObject> roads = new List<GameObject>();

    [SerializeField] GameObject mainRoadParent;
    List<GameObject> mainRoads = new List<GameObject>();

    [SerializeField] GameObject borderRoadParent;
    List<GameObject> borderRoads = new List<GameObject>();

    private void Start()
    {
        GenerateRoadGrid();
        AdjustCameraToFitGrid();
    }

    void GenerateRoadGrid()
    {
        for(int i = 0; i < levelArea.x; i++)
        {
            for(int j = 0; j < levelArea.y; j++)
            {
                GameObject road;

                if (j == mainRoadsZ)
                {
                    road = Instantiate(mainRoadPref, new Vector3(i, 0, j), Quaternion.identity);
                    road.transform.parent = mainRoadParent.transform;
                    mainRoads.Add(road);
                }
                else if (i == 0 || i == levelArea.x - 1 || j == 0 || j == levelArea.y - 1)
                {
                    road = Instantiate(borderRoadPref, new Vector3(i, 0, j), Quaternion.identity);
                    road.transform.parent = borderRoadParent.transform;
                    borderRoads.Add(road);
                }
                else
                {
                    road = Instantiate(roadPref, new Vector3(i, 0, j), Quaternion.identity);
                    road.transform.parent = roadParent.transform;
                    roads.Add(road);
                }
            }
        }
    }
    void AdjustCameraToFitGrid()
    {
        Camera mainCamera = Camera.main;

        int x = levelArea.x - 1;
      

        Vector3 gridCenter = new Vector3(x / 2f, 0, levelArea.y / 2f);

        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = levelArea.y / 2f;

            float aspectRatio = mainCamera.aspect;
            float horizontalSize = (levelArea.x / 2f) / aspectRatio;
            if (horizontalSize > mainCamera.orthographicSize)
            {
                mainCamera.orthographicSize = horizontalSize;
            }

            mainCamera.transform.position = new Vector3(gridCenter.x, 10, gridCenter.z);  
        }
        else
        {
            float halfHeight = levelArea.y / 2f;
            float halfWidth = levelArea.x / 2f;

            float fovInRadians = mainCamera.fieldOfView * Mathf.Deg2Rad;

            float cameraDistanceVertical = halfHeight / Mathf.Tan(fovInRadians / 2f);

            float aspectRatio = mainCamera.aspect;
            float cameraDistanceHorizontal = halfWidth / (Mathf.Tan(fovInRadians / 2f) * aspectRatio);
            float cameraDistance = Mathf.Max(cameraDistanceVertical, cameraDistanceHorizontal);

            mainCamera.transform.position = new Vector3(gridCenter.x, cameraDistance, gridCenter.z);
        }

        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
    }


}
