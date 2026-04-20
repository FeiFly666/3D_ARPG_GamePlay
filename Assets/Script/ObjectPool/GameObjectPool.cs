using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace Assets.ObjectPool
{
    internal class GameObjectPool<T> : BasePool<T> where T : Component
    {
        private T prefab;
        private Transform root;


        public GameObjectPool(T prefab, int maxNum, int preWarmNum, Transform parent = null)
        {
            this.prefab = prefab;

            this.max_Size = maxNum;

            this.root = parent;

            if(preWarmNum > maxNum)
            {
                preWarmNum = maxNum;
            }

            PreWarm(preWarmNum);
        }
        private void PreWarm(int num)
        {
            for (int i = 1; i <= num; i++)
            {
                Release(CreateInstance());
            }
        }

        public override T CreateInstance()
        {
            T go = GameObject.Instantiate(prefab, root);

            return go;
        }
        public override void OnGet(T item)
        {
            item.gameObject.SetActive(true);
        }
        public override void OnRelease(T item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(root,true);
        }
        public override void OnDestroyInstance(T item)
        {
            UObject.Destroy(item.gameObject);
        }
    }
}
