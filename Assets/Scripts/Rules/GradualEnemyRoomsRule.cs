using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "GradualEnemyRoomsRule", menuName = "Rules/GradualEnemyRoomsRule")]
    public class GradualEnemyRoomsRule : Rule
    {
        private Node _offender;
        
        public override bool IsEnforced(GraphCreator gc)
        {
            var nodeList = gc.BreadthFirstSearch(RoomType.MixedEnemy);

            foreach (var node in nodeList)
            {
                var parents = new List<Node>();
                var currentNode = node;

                while (currentNode.Parent != null)
                {
                    parents.Add(currentNode.Parent);
                    currentNode = currentNode.Parent;
                }

                if (parents.Any(n => n.RoomType == RoomType.MeleeEnemy) &&
                    parents.Any(n => n.RoomType == RoomType.RangedEnemy)) continue;

                _offender = node;
                return false;
            }

            return true;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            var neededRooms = new List<RoomType> { RoomType.MeleeEnemy, RoomType.RangedEnemy };
            var currentNode = _offender;
            
            while (currentNode.Children.Count > 0)
            {
                currentNode = currentNode.Children[Random.Range(0, currentNode.Children.Count)];
                if (currentNode.RoomType != RoomType.Boss) continue;
                
                currentNode = currentNode.Parent;
                break;
            }

            foreach (var roomType in neededRooms)
            {
                currentNode = gc.CreateChild(currentNode, roomType);
                currentNode.Lock();
            }

            _offender.SetType(currentNode.RoomType);
            currentNode.SetType(RoomType.MixedEnemy);
            
            _offender.Lock();
            currentNode.Lock();
        }
    }
}