using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour, IOnStart
{
    [SerializeField] protected Vector2Int spawnPoint;
    [SerializeField] protected LevelController levelController;

    public Vector2Int GetSpawnPoint()
    {
        return spawnPoint;
    }

    public virtual void OnStart()
    {

    }

    public virtual void SetLevelController(LevelController levelController)
    {
        this.levelController = levelController;
    }

    public Vector3 GetTransformPosition() { return transform.position; }
}
