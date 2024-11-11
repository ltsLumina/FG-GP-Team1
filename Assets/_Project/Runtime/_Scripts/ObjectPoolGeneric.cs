using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolStaticBatch : MonoBehaviour
{
    public static ObjectPoolStaticBatch Instance;

    private Dictionary<string, List<GameObject>> _pools;
    private Dictionary<string, GameObject> _prefabs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        
        Instance = this;
        _pools = new Dictionary<string, List<GameObject>>();
        _prefabs = new Dictionary<string, GameObject>();
    }

    public void CreateNewPool(string objectName, GameObject prefab)
    {
        if (!_pools.ContainsKey(objectName))
        {
            Debug.Log("Object pool already exists");
            return;
        }

        List<GameObject> list = new List<GameObject>();
        _pools.Add(objectName, list);
        _prefabs.Add(objectName, prefab);
    }

    public GameObject GetObject(string objectName)
    {
        if (_pools[objectName] == null)
        {
            Debug.Log("Object pool DNE of name " + objectName);
            return null;
        }

        GameObject newObject;

        if (_pools[objectName].Count == 0)
        {
            newObject = CreateNewObject(objectName);
            return newObject;
        }

        newObject = _pools[objectName][0];
        _pools[objectName].RemoveAt(0);

        return newObject;
    }

    private GameObject CreateNewObject(string objectName)
    {
        return Instantiate(_prefabs[objectName], new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void AddToPool(string objectName, GameObject addedObject)
    {
        if (_pools[objectName] == null)
        {
            Debug.Log("Cannot add to pool as object pool of name " + objectName + "DNE.");
        }

        addedObject.SetActive(false);
        _pools[objectName].Add(addedObject);
    }
}
