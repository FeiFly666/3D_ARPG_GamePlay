using Assets.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private AssetBundlePool _AssetBundlePool;

    private Dictionary<Type, object> _ClassPool = new Dictionary<Type, object>();

    private Dictionary<int, object> _GameObjectPool = new Dictionary<int, object>();
    private Dictionary<int,int> _instance2Prefab = new Dictionary<int, int>();

    private Transform root;
    private void Awake()
    {
        _AssetBundlePool = this.GetComponent<AssetBundlePool>();
        if( _AssetBundlePool == null )
        {
            _AssetBundlePool = this.AddComponent<AssetBundlePool>();
        }
        GameObject go = new GameObject("PoolRoot");
        
        go.transform.SetParent(transform);

        root = go.transform;
    }

    //=================================普通类对象池====================================

    ///<summary>
    ///普通类对象池获取对象
    /// </summary>

    public T GetClass<T>() where T : class, new()
    {
        object poolObj = null;
        Type type = typeof(T);

        if(!_ClassPool.TryGetValue(type, out poolObj))
        {
            var pool = new ClassPool<T>();
            
            _ClassPool.Add(type, pool);

            poolObj = pool;
        }

        return (poolObj as ClassPool<T>).Get();
    }
    ///<summary>
    ///普通类对象池回收对象
    /// </summary>
    public void ReleaseClass<T>(T item) where T : class,new()
    {
        if(_ClassPool.TryGetValue(typeof(T), out var poolObj))
        {
            (poolObj as ClassPool<T>).Release(item);
        }
    }

    //=================================游戏物体对象池====================================
    
    ///<summary>
    /// 创建一个新游戏物体对象池
    /// </summary>
    public void CreateGameObjectPool<T>(T prefab, int maxNum, int preWarmNum) where T : Component
    {
        int prefabID = prefab.gameObject.GetInstanceID();
        if (_GameObjectPool.ContainsKey(prefabID)) return;

        GameObjectPool<T> pool = new GameObjectPool<T>(prefab, maxNum, preWarmNum, root);

        _GameObjectPool.Add(prefab.gameObject.GetInstanceID(), pool);
    }
    ///<summary>
    /// 获取游戏对象（prefabID为Key）
    /// </summary>
    public T GetGameObject<T>(int prefabID ) where T : Component
    {
        T obj = null;
        if(_GameObjectPool.TryGetValue(prefabID, out var poolObj))
        {
            obj = (poolObj as GameObjectPool<T>).Get();

            int objID = obj.gameObject.GetInstanceID();

            if (!_instance2Prefab.ContainsKey(objID))
            {
                _instance2Prefab.Add(objID, prefabID);
            }
            else
            {
                _instance2Prefab[objID] = prefabID;
            }
        }

        return obj;
    }
    ///<summary>
    /// 回收游戏对象
    /// </summary>
    public void ReleaseGameObject<T>(T item) where T : Component
    {
        int prefabID = -1;
        if(!_instance2Prefab.TryGetValue(item.gameObject.GetInstanceID(), out prefabID))
        {
            Destroy(item.gameObject);
            return;
        }

        if(_GameObjectPool.TryGetValue(prefabID,out var poolObj))
        {
            (poolObj as GameObjectPool<T>).Release(item);

        }
        else
        {
            Destroy(item.gameObject);
            return;
        }
    }

    //=================================AssetBundle对象池====================================

    ///<summary>
    /// 获取一个AssetBundle
    /// </summary>
    public AssetBundle GetAssetBundle(string bundleName)
    {
        return _AssetBundlePool.LoadAsset(bundleName);
    }
    ///<summary>
    /// 回收一个AssetBundle
    /// </summary>
    public void ReleaseAssetBundle(string bundleName)
    {
        _AssetBundlePool.Release(bundleName);
    }

    //=================================统一清理====================================

    /// <summary>
    /// 释放所有内存资源
    /// </summary>
    public void ClearAll()
    {
        _instance2Prefab.Clear();
        _ClassPool.Clear();

        foreach (var pool in _GameObjectPool.Values)
        {
            (pool as IPool).Clear();
        }
        _GameObjectPool.Clear();

        _AssetBundlePool.ClearAll();
        Debug.Log("[PoolManager] 所有对象池已清空");
    }
}