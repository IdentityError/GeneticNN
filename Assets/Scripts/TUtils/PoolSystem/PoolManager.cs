using System;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance = null;

    public static PoolManager GetInstance()
    {
        return instance;
    }

    public PoolCategory[] poolsCategory;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        int length = poolsCategory.Length;
        for (int i = 0; i < length; i++)
        {
            poolsCategory[i].InitializePools(transform.position);
        }
    }

    /// <summary>
    ///   Spawn from the specified pool inside the specified category
    /// </summary>
    public GameObject Spawn(string categoryName, string poolTag, Vector3 position, Quaternion rotation)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if (poolCategory != null)
        {
            return poolCategory.SpawnFromPool(poolTag, position, rotation);
        }
        return null;
    }

    /// <summary>
    ///   Spawn from a random Pool inside the specified category based on Pools spawn probability
    /// </summary>
    public GameObject Spawn(string categoryName, Vector3 position, Quaternion rotation)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if (poolCategory != null)
        {
            return poolCategory.SpawnFromPool(null, position, rotation);
        }
        return null;
    }

    /// <summary>
    ///   Returns: a random poolTag from a specified category
    /// </summary>
    public string GetRandomCategoryPoolTag(string categoryName)
    {
        PoolCategory poolCategory = Array.Find(poolsCategory, category => category.name == categoryName);
        if (poolCategory != null)
        {
            return poolCategory.GetRandomPoolTag();
        }
        return null;
    }

    /// <summary>
    ///   Deactivate a object instead of destroying it. Super important in the Object Pooling paradigm
    /// </summary>
    public void DeactivateObject(GameObject objectToDeactivate)
    {
        objectToDeactivate.SetActive(false);
    }
}