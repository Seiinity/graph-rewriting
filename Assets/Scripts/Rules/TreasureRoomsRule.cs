using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "TreasureRoomsRule", menuName = "Rules/TreasureRoomsRule")]
    public class TreasureRoomsRule : Rule
    {
        private int _toAdd;
        
        public override bool IsEnforced(GraphCreator gc)
        {
            var totalRooms = gc.Graph.SelectMany(x => x).Count();
            _toAdd = Mathf.FloorToInt(totalRooms / 7f) - gc.Graph.SelectMany(x => x).Count(n => n.RoomType == RoomType.Treasure);
            return _toAdd == 0;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            switch (_toAdd)
            {
                case < 0:
                {
                    var availableNodes = gc.Graph.SelectMany(x => x).Where(n => !n.IsLocked && n.RoomType == RoomType.Treasure).ToList();
                    var randomisableTypes = new List<RoomType> { RoomType.MeleeEnemy, RoomType.RangedEnemy };
                
                    while (_toAdd < 0)
                    {
                        var randomNode = availableNodes[Random.Range(0, availableNodes.Count)];
                        availableNodes.Remove(randomNode);
                        randomNode.SetType(randomisableTypes[Random.Range(0, randomisableTypes.Count)]);
                
                        _toAdd++;
                    }

                    break;
                }
                case > 0:
                {
                    var availableNodes = gc.Graph.SelectMany(x => x).Where(n => !n.IsLocked && n.RoomType != RoomType.Treasure).ToList();
                    var possible = Mathf.Min(availableNodes.Count, _toAdd);
            
                    for (var i = 0; i < possible; i++)
                    {
                        var chosenNode = availableNodes[Random.Range(0, availableNodes.Count)];
                        availableNodes.Remove(chosenNode);
                        chosenNode.SetType(RoomType.Treasure);
                        chosenNode.Lock();
                
                        _toAdd--;
                    }

                    while (_toAdd > 0)
                    {
                        var levelToAddTo = gc.Graph.SelectMany(x => x).First(n => n.RoomType == RoomType.Boss).Parent.Level;
                        var randomParent = gc.Graph[levelToAddTo][Random.Range(0, gc.Graph[levelToAddTo].Count)];
                        var newNode = gc.CreateChild(randomParent, RoomType.Treasure);
                        newNode.Lock();
                
                        _toAdd--;
                    }

                    break;
                }
            }
        }
    }
}