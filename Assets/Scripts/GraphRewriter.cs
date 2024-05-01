using System.Collections.Generic;
using System.Linq;
using GraphRewriting.Rules;
using UnityEngine;

namespace GraphRewriting
{
    [RequireComponent(typeof(GraphCreator))]
    public class GraphRewriter : MonoBehaviour
    {
        [field: SerializeField]
        public List<Material> MaterialPerRoomType { get; private set; }

        [field: SerializeField]
        public List<Rule> Rules { get; private set; }

        private GraphCreator _gc;

        private void Awake()
        {
            _gc = GetComponent<GraphCreator>();
        }

        private void Start()
        {
            EnforceRules();
            _gc.DrawGraph();
        }

        private void EnforceRules()
        {
            foreach (var rule in Rules.Where(rule => !rule.IsEnforced(_gc)))
            {
                rule.EnforceRule(_gc);
                EnforceRules();
                return;
            }
        }

        public void UpdateNodeMaterial(Node node)
        {
            node.GetComponent<MeshRenderer>().material = MaterialPerRoomType[(int)node.RoomType];
        }
    }
}