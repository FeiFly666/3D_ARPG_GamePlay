using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.ObjectPool
{
    public abstract class BasePool<T> : IPool where T : class
    {
        protected Stack<T> stack = new Stack<T>();

        protected int max_Size = 100;

        public int Count => stack.Count;

        public virtual T Get()
        {
            T item = null;
            if(stack.Count > 0)
            {
                item = stack.Pop();
            }
            else
            {
                //Debug.Log("<color=cyan>对象池剩余对象不足，创建新对象</color>");
                item = CreateInstance();
            }

            OnGet(item);

            return item;
        }

        public virtual void Release(T obj)
        {
            if (obj == null) return;
            if(stack.Contains(obj))
            {
                Debug.LogError("禁止重复Release相同对象");
                return;
            }

            OnRelease(obj);

            if(stack.Count < max_Size)
            {
                stack.Push(obj);
            }
            else
            {
                OnDestroyInstance(obj);
            }
        }

        public virtual void Clear()
        {
            while(stack.Count > 0)
            {
                OnDestroyInstance(stack.Pop());
            }
        }


        //子类自行实现的函数
        public abstract T CreateInstance();
        public virtual void OnGet(T item) { }
        public virtual void OnRelease(T item) { }

        public virtual void OnDestroyInstance(T item) { }

    }
}
