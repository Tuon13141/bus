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
        path += level.ToString();
        GameObject levelObj = Resources.Load<GameObject>(path);
        currentLevelObject = Instantiate(levelObj);
        inputManager.CanClick = true;
    }

    public void Retry()
    {
        Destroy(currentLevelObject);
        GameObject levelObj = Resources.Load<GameObject>(path);
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
        Destroy(currentLevelObject);
        GameObject levelObj = Resources.Load<GameObject>(path);
        currentLevelObject = Instantiate(levelObj);
        UIWin.SetActive(false);
        inputManager.CanClick = true;
    }

    public void Win()
    {
        inputManager.CanClick = false;
        UIWin.SetActive(true);
    }

    public void Lose()
    {
        inputManager.CanClick = false;
        UILose.SetActive(true);
    }
}
