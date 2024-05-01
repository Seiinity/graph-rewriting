using System.Collections.Generic;
using UnityEngine;

namespace GraphRewriting
{
    public class Node : MonoBehaviour
    {
        public int Level { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; private set; }
        public RoomType RoomType { get; private set; }
        public bool IsLocked { get; private set; }

        private void Awake() 
        {
            Children = new List<Node>();
            IsLocked = false;
        }

        private void OnDestroy()
        {
            if (Parent == null) return;

            Parent.Children.Remove(this);
            Parent.GetComponent<LineController>().UpdatePoints();
        }

        public void SetType(RoomType type)
        {
            RoomType = type;
            transform.parent.GetComponent<GraphRewriter>().UpdateNodeMaterial(this); // Hacky way!
        }

        public void Lock()
        {
            IsLocked = true;
        }

        private void OnDrawGizmos()
        {
            if (!IsLocked) return;
            
            Gizmos.color = new Color(1, 0, 1, .3f);
            Gizmos.DrawSphere(transform.position, 2.5f);
        }
    }
}
