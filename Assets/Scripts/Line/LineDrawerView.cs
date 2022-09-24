using System;
using System.Collections.Generic;
using UnityEngine;

namespace Line
{
    public class LineDrawerView : MonoBehaviour
    {
        [SerializeField] private LineView linePrefab;
        [SerializeField] private float pointsStep = 0.1f;

        private readonly List<Vector2> _drawPoints = new();

        private LineView _currentLine;
        private bool _canDraw = true;
        private Camera _camera;

        public event Action LineFinished;

        private void Update()
        {
            if (!_canDraw) return;
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100);

            if (hit.collider) return;
            
            if (Input.GetMouseButton(0))
            {
                if (!_currentLine)
                {
                    CreateNewLine();
                }

                Vector3 currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 lastPoint = _drawPoints[^1];

                RaycastHit2D hitLine = Physics2D.Linecast(currentMousePosition, lastPoint);

                if (hitLine.collider)
                {
                    return;
                }

                if (Vector2.Distance(currentMousePosition, lastPoint) < pointsStep) return;

                _drawPoints.Add(currentMousePosition);
                _currentLine.UpdateLine(_drawPoints);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _currentLine.EnablePhysicsSimulation();
                _currentLine = null;
                _canDraw = false;
                LineFinished?.Invoke();
            }
        }

        private void CreateNewLine()
        {
            _drawPoints.Clear();

            _drawPoints.Add(_camera.ScreenToWorldPoint(Input.mousePosition));
            _drawPoints.Add(_camera.ScreenToWorldPoint(Input.mousePosition));

            _currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);

            _currentLine.UpdateLine(_drawPoints);
        }

        private void Awake()
        {
            _camera = Camera.main;
        }
    }
}