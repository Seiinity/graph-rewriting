using System.Linq;
using UnityEngine;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "MinimumHealRoomsRule", menuName = "Rules/MinimumHealRoomsRule")]
    public class MinimumHealRoomsRule : Rule
    {
        private int _toAdd;
        
        public override bool IsEnforced(GraphCreator gc)
        {
            var totalRooms = gc.Graph.SelectMany(x => x).Count();
            _toAdd = Mathf.FloorToInt(totalRooms / 5f) - gc.Graph.SelectMany(x => x).Count(n => n.RoomType == RoomType.Healing);
            return _toAdd <= 0;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            var availableNodes = gc.Graph.SelectMany(x => x).Where(n => !n.IsLocked && n.RoomType != RoomType.Healing).ToList();
            var possible = Mathf.Min(availableNodes.Count, _toAdd);
            
            for (var i = 0; i < possible; i++)
            {
                var chosenNode = availableNodes[Random.Range(0, availableNodes.Count)];
                availableNodes.Remove(chosenNode);
                chosenNode.SetType(RoomType.Healing);
                chosenNode.Lock();
                
                _toAdd--;
            }

            while (_toAdd > 0)
            {
                var levelToAddTo = gc.Graph.SelectMany(x => x).First(n => n.RoomType == RoomType.Boss).Parent.Level;
                var randomParent = gc.Graph[levelToAddTo][Random.Range(0, gc.Graph[levelToAddTo].Count)];
                var newNode = gc.CreateChild(randomParent, RoomType.Healing);
                newNode.Lock();
                
                _toAdd--;
            }
        }
    }
}