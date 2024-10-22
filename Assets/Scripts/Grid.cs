using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour, IOnStart
{
    [SerializeField] bool destroyOnNewLoad = true;
    public bool DestroyOnNewLoad => destroyOnNewLoad;
    [SerializeField] protected Vector2Int spawnPoint;
    protected LevelController levelController;

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

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void UnActiveSelf()
    {
        gameObject.SetActive(false);
    }
    public Vector3 GetTransformPosition() { return new Vector3(transform.position.x, 0, transform.position.z); }

    public virtual void ResetParameter()
    {
        
    }
}
