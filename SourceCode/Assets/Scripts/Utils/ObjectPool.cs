using System;
using System.Collections.Generic;

namespace mehmetsrl.Utils.DesignPatterns
{
    /// <summary>
    /// Unityies generic implementation of object pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PooledObject<T> : IDisposable where T : class
    {
        private readonly T m_ToReturn;

        private readonly IObjectPool<T> m_Pool;

        internal PooledObject(T value, IObjectPool<T> pool)
        {
            m_ToReturn = value;
            m_Pool = pool;
        }

        void IDisposable.Dispose()
        {
            m_Pool.Release(m_ToReturn);
        }
    }


    public interface IObjectPool<T> where T : class
    {
        int CountInactive
        {
            get;
        }

        T Get();

        PooledObject<T> Get(out T v);

        void Release(T element);

        void Clear();
    }


    //
    // Summary:
    //     A stack based Pool.IObjectPool_1.
    public class ObjectPool<T> : IDisposable, IObjectPool<T> where T : class
    {
        internal readonly Stack<T> m_Stack;

        private readonly Func<T> m_CreateFunc;

        private readonly Action<T> m_ActionOnGet;

        private readonly Action<T> m_ActionOnRelease;

        private readonly Action<T> m_ActionOnDestroy;

        private readonly int m_MaxSize;

        internal bool m_CollectionCheck;

        public int CountAll
        {
            get;
            private set;
        }

        public int CountActive => CountAll - CountInactive;

        public int CountInactive => m_Stack.Count;

        public ObjectPool(Func<T> createFunc, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            if (createFunc == null)
            {
                throw new ArgumentNullException("createFunc");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("Max Size must be greater than 0", "maxSize");
            }

            m_Stack = new Stack<T>(defaultCapacity);
            m_CreateFunc = createFunc;
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_ActionOnDestroy = actionOnDestroy;
            m_CollectionCheck = collectionCheck;
        }

        public T Get()
        {
            T val;
            if (m_Stack.Count == 0)
            {
                val = m_CreateFunc();
                CountAll++;
            }
            else
            {
                val = m_Stack.Pop();
            }

            m_ActionOnGet?.Invoke(val);
            return val;
        }

        public PooledObject<T> Get(out T v)
        {
            return new PooledObject<T>(v = Get(), this);
        }

        public void Release(T element)
        {
            if (m_CollectionCheck && m_Stack.Count > 0 && m_Stack.Contains(element))
            {
                throw new InvalidOperationException("Trying to release an object that has already been released to the pool.");
            }

            m_ActionOnRelease?.Invoke(element);
            if (CountInactive < m_MaxSize)
            {
                m_Stack.Push(element);
            }
            else
            {
                m_ActionOnDestroy?.Invoke(element);
            }
        }

        public void Clear()
        {
            if (m_ActionOnDestroy != null)
            {
                foreach (T item in m_Stack)
                {
                    m_ActionOnDestroy(item);
                }
            }

            m_Stack.Clear();
            CountAll = 0;
        }

        public void Dispose()
        {
            Clear();
        }
    }
}