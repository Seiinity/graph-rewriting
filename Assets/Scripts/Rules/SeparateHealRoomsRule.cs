using System.Linq;
using UnityEngine;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "SeparateHealRoomsRule", menuName = "Rules/SeparateHealRoomsRule")]
    public class SeparateHealthRoomsRule : Rule
    {
        private Node _offender;
        
        public override bool IsEnforced(GraphCreator gc)
        {
            var nodeList = gc.BreadthFirstSearch(RoomType.Healing);

            foreach (var node in nodeList.Where(node => node.Parent.RoomType == RoomType.Healing))
            {
                _offender = node;
                return false;
            }

            return true;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            var randomisableTypes = gc.RandomisableTypes.Where(t => t != RoomType.Healing && t != RoomType.MixedEnemy).ToList();
            
            if (_offender.Parent.IsLocked) _offender.SetType(randomisableTypes[Random.Range(0, randomisableTypes.Count)]);
            else _offender.Parent.SetType(randomisableTypes[Random.Range(0, randomisableTypes.Count)]);
        }
    }
}
