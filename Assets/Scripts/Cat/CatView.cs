using System;
using Bee;
using Land;
using UnityEngine;

namespace Cat
{
    public class CatView : MonoBehaviour
    {
        private Transform _transform;
        
        public event Action Died;

        public Transform GetTransform() => _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log("hit by " + col.gameObject.name);

            if (col.gameObject.GetComponent<BeeView>())
            {
                Died?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.GetComponent<WaterView>() != null)
            {
                Died?.Invoke();
            }
        }
    }
}