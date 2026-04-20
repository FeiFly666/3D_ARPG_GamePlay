using System;

namespace Assets.ObjectPool
{
    public interface IPoolable
    {
        public void OnSpawn();
        public void OnDeSpawn();
    }
}
