using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Utils.ObjectPool
{
    public class PoolBase<T>
    {
        private readonly Func<T> _preloadFunc;
        private readonly Action<T> _getAction;
        private readonly Action<T> _returnAction;

        private Queue<T> _pool = new();

        public PoolBase(Func<T> preloadFunc, Action<T> getAction, Action<T> returnAction, int preloadCount)
        {
            _preloadFunc = preloadFunc;
            _getAction = getAction;
            _returnAction = returnAction;

            if (_preloadFunc == null)
            {
                Debug.LogError("Preload function is null");
                return;
            }

            for (int i = 0; i < preloadCount; i++)
            {
                Return(_preloadFunc());
            }
        }

        public T Get()
        {
            T item = _pool.Count > 0 ? _pool.Dequeue() : _preloadFunc();

            // if (_pool.Count <= 0)
            // {
            //     _preloadFunc();
            // }
            //
            // T item = _pool.Dequeue();
            _getAction(item);

            return item;
        }

        public void Return(T item)
        {
            _returnAction(item);
            _pool.Enqueue(item);
        }

        // public void ReturnAll()
        // {
        //     foreach (T item in _active)
        //     {
        //         Return(item);
        //     }
        // }
    }
}