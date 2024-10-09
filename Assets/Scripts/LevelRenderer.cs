using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    [SerializeField] GameObject mainRoadPref;
    [SerializeField] GameObject roadPref;
    [SerializeField] GameObject borderRoadPref;

    [SerializeField] Vector2Int levelArea;
  

    [SerializeField] int mainRoadsZ;

    [SerializeField] GameObject roadParent;
    List<GridRoad> roads = new List<GridRoad>();

    [SerializeField] GameObject mainRoadParent;
    List<GridMainRoad> mainRoads = new List<GridMainRoad>();

    [SerializeField] GameObject borderRoadParent;
    List<GridBorderRoad> borderRoads = new List<GridBorderRoad>();

    [SerializeField] LevelController levelController;

    private void Start()
    {
        GenerateRoadGrid();
        AdjustCameraToFitGrid();
    }

    void GenerateRoadGrid()
    {
        int halfX = levelArea.x / 2;
        int halfY = levelArea.y / 2;

        for(int i = -halfX; i < halfX; i++)
        {
            for(int j = -halfY; j < halfY; j++)
            {
                GameObject roadObj;
                Vector2Int spawnPoint = new Vector2Int(i, j);
                Vector3 location = new Vector3(i, -0.1f, j);

                if (j == (int) mainRoadsZ / 2)
                {
                    roadObj = Instantiate(mainRoadPref, location, Quaternion.identity);
                    roadObj.transform.parent = mainRoadParent.transform;
                    GridMainRoad road = roadObj.GetComponent<GridMainRoad>();
                    road.SetUp(spawnPoint, levelController);
                    mainRoads.Add(road);
                }
                else if (i == -halfX || i == halfX - 1 || j == -halfY || j == halfY - 1)
                {
                    roadObj = Instantiate(borderRoadPref, location, Quaternion.identity);
                    roadObj.transform.parent = borderRoadParent.transform;
                    GridBorderRoad road = roadObj.GetComponent<GridBorderRoad>();
                    road.SetUp(spawnPoint, levelController);
                    borderRoads.Add(road);
                }
                else
                {
                    roadObj = Instantiate(roadPref, location, Quaternion.identity);
                    roadObj.transform.parent = roadParent.transform;
                    GridRoad road = roadObj.GetComponent<GridRoad>();
                    road.SetUp(spawnPoint, levelController);
                    roads.Add(road);
                }
            }
        }

        //Debug.Log(levelController.RoadDict.Count);
    }
    void AdjustCameraToFitGrid()
    {
        Camera mainCamera = Camera.main;

        float x = -0.5f;
        if (levelArea.x % 2 != 0) {
            x = -0.5f;
        }
      

        Vector3 gridCenter = new Vector3(x, 0, 0);

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
