using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.ObjectPool
{
    public class AssetBundleRecord
    {
        public string bundleName;

        public AssetBundle assetBundle;

        public int count;

        public bool waitForUnload = false;

        public float startWaitTime = -1f;

        public AssetBundleRecord(string bundleName, AssetBundle assetBundle)
        {
            this.bundleName = bundleName;
            this.assetBundle = assetBundle;
            this.count = 1;
        }

        public void AddCount()
        {
            this.count++;
        }
        public void RemoveCount() { this.count--;}

        public void Unload()
        {
            if(assetBundle != null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
        }
    }
    //没做完呢！！！！！！！！！！！！！！！！
    public class AssetBundlePool : MonoBehaviour
    {
        private Dictionary<string, AssetBundleRecord> assetBundleCounts = new Dictionary<string, AssetBundleRecord>(101);

        private Queue<AssetBundleRecord> waitForUnload = new Queue<AssetBundleRecord>();

        public float totalWaitTime = 60f;

        private void Update()
        {
            CheckWaitTime();
        }
        private void CheckWaitTime()
        {
            if(waitForUnload.Count > 0)
            {
                AssetBundleRecord record = waitForUnload.Peek();
                if (record.waitForUnload && Time.time - record.startWaitTime >= totalWaitTime)
                {
                    record.assetBundle.Unload(false);
                    waitForUnload.Dequeue();
                }
            }
        }

        public AssetBundle LoadAsset(string bundleName)
        {
            return null;
        }

        public void Release(string bundleName)
        {
            if(assetBundleCounts.TryGetValue(bundleName, out var record))
            {
                record.RemoveCount();

                Debug.Log($"{bundleName}引用计数减一，目前引用数量：{record.count}");

                if(record.count <= 0)
                {
                    Debug.Log($"{bundleName}无引用，加入待卸载队列");
                    waitForUnload.Enqueue(record);
                }
            }
        }

        public void ClearAll()
        {
            foreach(var record in assetBundleCounts.Values)
            {
                record.Unload();
            }

            assetBundleCounts.Clear();
        }

    }
}
