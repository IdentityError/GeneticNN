// Copyright (c) 2020 Matteo Beltrame

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TUtils.ObjectPooling
{
    public class PoolManager : MonoBehaviour
    {
        private static PoolManager instance = null;

        public static PoolManager GetInstance()
        {
            return instance;
        }

        [SerializeField] private PoolCategory[] poolsCategory;

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

        #region PoolCategory

        /// <summary>
        ///   Class representing a pool category
        /// </summary>
        [System.Serializable]
        private class PoolCategory
        {
            public string name;
            public Pool[] pools;
            public Dictionary<string, Queue<GameObject>> poolsDictionary;

            /// <summary>
            ///   Initialize the pools
            /// </summary>
            public void InitializePools(Vector3 position)
            {
                poolsDictionary = new Dictionary<string, Queue<GameObject>>();
                int length = pools.Length;
                for (int i = 0; i < length; i++)
                {
                    Queue<GameObject> objectPool = new Queue<GameObject>();
                    int poolDim = pools[i].poolSize;
                    for (int j = 0; j < poolDim; j++)
                    {
                        GameObject obj = GameObject.Instantiate(pools[i].prefab, position, Quaternion.identity);
                        obj.SetActive(false);
                        objectPool.Enqueue(obj);
                    }
                    poolsDictionary.Add(pools[i].tag, objectPool);
                }
                NormalizeSpawnProbabilities();
            }

            /// <summary>
            ///   <para>
            ///     Spawn a GameObject from a specified pool, if poolTag is null the object will be selected based on the pool probability
            ///   </para>
            ///   Returns: the pooled object
            /// </summary>
            public GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion rotation)
            {
                if (poolTag == null)
                {
                    poolTag = GetRandomPoolTag();
                }
                if (poolsDictionary[poolTag] == null)
                {
                    return null;
                }
                if (!poolsDictionary.ContainsKey(poolTag))
                {
                    return null;
                }

                IPooledObject[] poolInterfaces;
                GameObject objectToSpawn = poolsDictionary[poolTag].Dequeue();
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;

                poolInterfaces = objectToSpawn.GetComponentsInChildren<IPooledObject>();
                objectToSpawn.SetActive(true);
                if (poolInterfaces != null)
                {
                    int length = poolInterfaces.Length;
                    for (int i = 0; i < length; i++)
                    {
                        poolInterfaces[i].OnObjectSpawn();
                    }
                }
                poolsDictionary[poolTag].Enqueue(objectToSpawn);
                return objectToSpawn;
            }

            /// <summary>
            ///   Returns: a random poolTag based on pools probabilities
            /// </summary>
            public string GetRandomPoolTag()
            {
                int index = -1;
                float radix = UnityEngine.Random.Range(0f, 1f);
                int count = pools.Length;
                while (radix > 0 && index < count)
                {
                    radix -= pools[++index].spawnProbability;
                }
                return pools[index].tag;
            }

            private void NormalizeSpawnProbabilities()
            {
                float total = 0f;
                int length = pools.Length;
                for (int i = 0; i < length; i++)
                {
                    total += pools[i].spawnProbability;
                }
                for (int i = 0; i < length; i++)
                {
                    pools[i].spawnProbability /= total;
                }
            }
        }

        #endregion PoolCategory
    }
}