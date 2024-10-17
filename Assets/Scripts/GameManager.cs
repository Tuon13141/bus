using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] LevelController levelController;
    [SerializeField] InputManager inputManager;
    [SerializeField] GameObject UIWin;
    [SerializeField] GameObject UILose;

    [SerializeField] int level = 1;
    int maxLevel = 4;
    string path = "Levels/Level_";

    GameObject currentLevelObject;

    private void Start()
    {
        GameObject levelObj = Resources.Load<GameObject>(GetLevelPath());
        currentLevelObject = Instantiate(levelObj);
        inputManager.CanClick = true;
    }

    public void Retry()
    {
        Destroy(currentLevelObject);
        GameObject levelObj = Resources.Load<GameObject>(GetLevelPath());
        currentLevelObject = Instantiate(levelObj);
        UILose.SetActive(false);
        inputManager.CanClick = true;
    }

    public void NextLevel()
    {
        level++;
        if (level > maxLevel)
        {
            level = 1;
        }
       
        GameObject levelObj = Resources.Load<GameObject>(GetLevelPath());
        currentLevelObject = Instantiate(levelObj);
        UIWin.SetActive(false);
        inputManager.CanClick = true;
    }

    public void Win()
    {
        inputManager.CanClick = false;
        Destroy(currentLevelObject);
        UIWin.SetActive(true);
    }

    public void Lose()
    {
        inputManager.CanClick = false;
        Destroy(currentLevelObject);
        UILose.SetActive(true);
    }

    string GetLevelPath()
    {
        return path + level;
    }
}
