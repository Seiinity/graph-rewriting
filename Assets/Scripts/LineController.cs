using System.Collections.Generic;
using UnityEngine;

namespace GraphRewriting
{
    public class LineController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private List<Transform> _points;
        private Node _node;

        private void Awake()
        {
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            
            _points = new List<Transform>();

            _node = GetComponent<Node>();
        }

        private void Update()
        {
            for (var i = 0; i < _points.Count; i++)
            {
                _lineRenderer.SetPosition(i, _points[i].position);
            }
        }

        public void UpdatePoints()
        {
            _points.Clear();

            foreach (var child in _node.Children)
            {
                AddPoint(_node.transform);
                AddPoint(child.transform);
            }
        }

        private void AddPoint(Transform point)
        {
            _lineRenderer.positionCount = _points.Count + 1;
            _points.Add(point);
        }
    }
}