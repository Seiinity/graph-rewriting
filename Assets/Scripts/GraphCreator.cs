using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GraphRewriting
{
    public class GraphCreator : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject NodePrefab { get; private set; }
        
        [field: SerializeField]
        public int Levels { get; private set; }
        
        [field: SerializeField]
        public int MaxChildren { get; private set; }
        
        [field: SerializeField]
        public float ChildrenDecay { get; private set; }
        
        [field: SerializeField]
        public float ChanceDecay { get; private set; }
        
        [field: SerializeField, Range(0, 100)]
        public float ChanceOfEdge { get; private set; }
        
        [field: SerializeField]
        public List<RoomType> RandomisableTypes { get; private set; }
        
        [field: SerializeField]
        public string Seed { get; private set; }

        public List<List<Node>> Graph { get; private set; }
        
        private Node _root;
        private int _currLevel;

        private void Awake()
        {
            Graph = new List<List<Node>>();
            _currLevel = 0;
            
            if (Seed.Length > 0) Random.InitState(Seed.GetHashCode());
            
            CreateGraph();
        }

        private void CreateGraph()
        {
            CreateRootNode();
            CreateLevels();
        }

        private void CreateRootNode()
        {
            _root = Instantiate(NodePrefab, transform).GetComponent<Node>();
            _root.Level = _currLevel;
            _root.SetType(RoomType.Entrance);
            _root.Lock();
            
            AddChildrenToList(_root);
        }

        private void CreateLevels()
        {
            while (_currLevel <= Levels - 1) CreateLevel();
        }

        private void CreateLevel()
        {
            foreach (var node in Graph[_currLevel]) CreateChildrenForNode(node);

            _currLevel++;
            MaxChildren = Mathf.FloorToInt(Mathf.Max(1, MaxChildren / ChildrenDecay));
        }

        private void CreateChildrenForNode(Node parent)
        {
            CreateChild(parent);

            for (var i = 0; i <= MaxChildren - 1; i++)
            {
                if (Random.Range(0, 100) <= ChanceOfEdge / ChanceDecay) CreateChild(parent);
            }
        }

        public Node CreateChild(Node parent, RoomType type = RoomType.Undefined)
        {
            var child = Instantiate(NodePrefab, transform).GetComponent<Node>();

            child.Parent = parent;
            child.Level = parent.Level + 1;

            if (type == RoomType.Undefined) type = RandomisableTypes[Random.Range(0, RandomisableTypes.Count)];
            child.SetType(type);
            parent.Children.Add(child);

            AddChildrenToList(child);
            
            parent.GetComponent<LineController>().UpdatePoints();

            return child;
        }

        private void AddChildrenToList(Node child)
        {
            if (Graph.Count <= child.Level) Graph.Add(new List<Node>());
            Graph[child.Level].Add(child);
        }

        public void DeleteNode(Node node)
        {
            Graph[node.Level].Remove(node);
            Destroy(node.gameObject);
        }

        public List<Node> BreadthFirstSearch(RoomType type)
        {
            var queue = new Queue<Node>();
            var visitedNodes = new HashSet<Node>();
            var resultList = new List<Node>();
            
            queue.Enqueue(_root);
            visitedNodes.Add(_root);

            while (queue.Count > 0)
            {
                var currentNode  = queue.Dequeue();
                if (currentNode.RoomType == type) resultList.Add(currentNode);
                
                foreach (var child in currentNode.Children.Where(child => !visitedNodes.Contains(child)))
                {
                    queue.Enqueue(child);
                    visitedNodes.Add(child);
                }
            }

            return resultList;
        }
        
        public void DrawGraph()
        {
            if (Graph == null || Graph.Count == 0) return;

            for (var level = 0; level < Graph.Count; level++)
            {
                var nodesAtLevel = Graph[level];

                if (level == 0) // Centers the root.
                {
                    nodesAtLevel[0].transform.position = new Vector3(0, 0, 0);
                    continue;
                }

                foreach (var node in nodesAtLevel)
                {
                    // Calculates the center position based on the parent's position.
                    if (node.Parent == null) continue;
                    
                    var siblingIndex = node.Parent.Children.IndexOf(node);
                    var totalWidth = node.Parent.Children.Sum(CalculateSubtreeWidth);
                    var startPosition = node.Parent.transform.position.x - totalWidth / 2;

                    var currentPosition = startPosition;
                    for (var i = 0; i < siblingIndex; i++) currentPosition += CalculateSubtreeWidth(node.Parent.Children[i]);

                    var horizontalPosition = currentPosition + CalculateSubtreeWidth(node) / 2;
                    node.transform.position = new Vector3(horizontalPosition, 0, -level * 10);
                }
            }
        }

        private static float CalculateSubtreeWidth(Node node)
        {
            return node.Children.Count == 0 ? 8f : node.Children.Sum(CalculateSubtreeWidth);
        }
    }
}