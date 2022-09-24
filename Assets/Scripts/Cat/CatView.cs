using System;
using UnityEngine;

namespace Cat
{
    public interface ICatView
    {
        void Kill();
    }

    public class CatView : MonoBehaviour, ICatView
    {
        private Transform _transform;
        
        public event Action Died;

        public Transform GetTransform() => _transform;

        private void Awake()
        {
            _transform = transform;
        }
        
        public void Kill() => Died?.Invoke();
    }
}