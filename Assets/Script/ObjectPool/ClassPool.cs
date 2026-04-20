using System;

namespace Assets.ObjectPool
{
    public class ClassPool<T> : BasePool<T> where T : class,new()
    {

        public override T CreateInstance()
        {
            return new T();
        }
        public override void Release(T obj)
        {
            base.Release(obj);
        }
    }
}
