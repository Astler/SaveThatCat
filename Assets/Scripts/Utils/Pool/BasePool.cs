using System.Collections.Generic;
using UnityEngine;

namespace Utils.Pool
{
    public class BasePool<T> : MonoBehaviour, IPool<T> where T : IPoolElement<BasePool<T>>
    {
        [SerializeField] private int poolSize = 15;
        [Space, SerializeField] private GameObject poolPrefab;

        private Transform _transform;
        private readonly Stack<T> _pool = new();

        private void CreatePoolElement()
        {
            GameObject instantiatedObject = Instantiate(poolPrefab, _transform.position, Quaternion.identity, _transform);
            instantiatedObject.SetActive(false);

            T poolObject = instantiatedObject.GetComponent<T>();
            _pool.Push(poolObject);
        }

        private void Awake()
        {
            _transform = transform;

            for (int i = 0; i < poolSize; i++)
            {
                CreatePoolElement();
            }
        }

        public T Spawn()
        {
            if (_pool.Count == 0) CreatePoolElement();

            T element = _pool.Pop();
            element.SetActive(true);
            return element;
        }

        public void Despawn(T element)
        {
            element.SetActive(false);
            _pool.Push(element);
        }
    }
}