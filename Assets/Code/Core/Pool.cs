using System.Collections.Generic;
using UnityEngine;

namespace ForestSlice.Core
{
    public class Pool<T> where T : MonoBehaviour
    {
        private T prefab;
        private Transform parent;
        private Queue<T> pool = new Queue<T>();
        private int initialSize;

        public Pool(T prefab, Transform parent, int initialSize = 10)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.initialSize = initialSize;

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private T CreateNewObject()
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }

        public T Get()
        {
            if (pool.Count == 0)
            {
                CreateNewObject();
            }

            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (var obj in pool)
            {
                if (obj != null)
                {
                    Object.Destroy(obj.gameObject);
                }
            }
            pool.Clear();
        }
    }
}
