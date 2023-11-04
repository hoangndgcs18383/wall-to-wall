using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolManager
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get { return _instance ??= new PoolManager(); }
    }

    private PoolManager()
    {
        Initialize();
    }

    private Transform _poolParent;
    
    private Dictionary<int, Queue<GameObject>> _poolList = new Dictionary<int, Queue<GameObject>>();

    public void Initialize()
    {
        _poolList.Clear();
        if(_poolParent == null)
            _poolParent = new GameObject("PoolParent").transform;
    }
    
    public void CreateOrGetPool(GameObject prefab, int count, Action<GameObject> callback)
    {
        int key = prefab.GetInstanceID();
        if (_poolList.ContainsKey(key))
        {
            callback?.Invoke(_poolList[key].Dequeue());
            
            if (_poolList[key].Count == 0)
            {
                GameObject obj = Object.Instantiate(prefab, _poolParent, true);
                obj.SetActive(false);
                _poolList[key].Enqueue(obj);
            }
        }
        else
        {
            _poolList.Add(key, new Queue<GameObject>());
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Object.Instantiate(prefab, _poolParent, true);
                obj.SetActive(false);
                _poolList[key].Enqueue(obj);
            }

            callback?.Invoke(_poolList[key].Dequeue());
        }
    }
    
    public IEnumerator ReturnPool(GameObject prefab, GameObject obj, float delay, bool isAutoDisable = false , Action<GameObject> callback = null)
    {
        yield return new WaitForSeconds(delay);
        ReturnPool(prefab, obj, isAutoDisable, callback);
    }

    public void ReturnPool(GameObject prefab, GameObject obj, bool isAutoDisable = false , Action<GameObject> callback = null)
    {
        int key = prefab.GetInstanceID();
        callback?.Invoke(obj);
        if(isAutoDisable) obj.SetActive(false);
        _poolList[key].Enqueue(obj);
    }
    
    public void ClearPool()
    {
        foreach (var pool in _poolList)
        {
            foreach (var obj in pool.Value)
            {
                Object.Destroy(obj);
            }
        }
        _poolList.Clear();
    }
}