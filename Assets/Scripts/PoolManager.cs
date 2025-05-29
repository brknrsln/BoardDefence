using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private List<Pool> pools;
    
    public static PoolManager Instance { get; private set; }

    private Dictionary<Constants.ObjectItemType, Queue<GameObject>> _poolDictionary;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        _poolDictionary = new Dictionary<Constants.ObjectItemType, Queue<GameObject>>();
        var levelData = Constants.LevelDataDictionary[PlayerData.Instance.Level];

        foreach (var pool in pools)
        {
            var objectPool = new Queue<GameObject>();

            for (var i = 0; i < levelData.TypeSizeDictionary[pool.objectItemType]; i++)
            {
                var obj = Instantiate(pool.prefab, transform, true);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            _poolDictionary.Add(pool.objectItemType, objectPool);
            Debug.Log($"Initialized pool for: {pool.objectItemType} with size: {levelData.TypeSizeDictionary[pool.objectItemType]}");
        }
    }
    
    public GameObject SpawnFromPool(Constants.ObjectItemType objectItemType, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(objectItemType))
        {
            Debug.LogWarning($"Pool with tag {objectItemType} doesn't exist.");
            return null;
        }

        if (_poolDictionary[objectItemType].Count == 0)
        {
            return null;
        }

        var objectToSpawn = _poolDictionary[objectItemType].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        var poolableObject = objectToSpawn.GetComponent<IPoolable>();
        poolableObject?.OnSpawnFromPool();

        return objectToSpawn;
    }

    public void ReturnToPool(Constants.ObjectItemType objectItemType, GameObject objectToReturn)
    {
        if (!_poolDictionary.ContainsKey(objectItemType))
        {
            Debug.LogWarning($"Pool with tag {objectItemType} doesn't exist.");
            Destroy(objectToReturn);
            return;
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform);
        _poolDictionary[objectItemType].Enqueue(objectToReturn);

        var poolableObject = objectToReturn.GetComponent<IPoolable>();

        poolableObject?.OnReturnToPool();
    }

    private bool IsInPool(Constants.ObjectItemType objectItemType)
    {
        return _poolDictionary[objectItemType].Count > 0;
    }
    
    public bool IsTotalEnemiesSpawned()
    {
        foreach (var enemyType in Constants.EnemyTypes)
        {
            if (IsInPool(enemyType))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public int GetCountOfTypeInPool(Constants.ObjectItemType objectItemType)
    {
        if (_poolDictionary.ContainsKey(objectItemType))
        {
            return _poolDictionary[objectItemType].Count;
        }
        
        Debug.LogWarning($"Pool with tag {objectItemType} doesn't exist.");
        return 0;
    }
}