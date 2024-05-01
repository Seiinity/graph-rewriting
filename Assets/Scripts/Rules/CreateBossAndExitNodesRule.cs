using System.Linq;
using UnityEngine;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "CreateBossAndExitNodesRule", menuName = "Rules/CreateBossAndExitNodesRule")]
    public class CreateBossAndExitNodesRule : Rule
    {
        public override bool IsEnforced(GraphCreator gc)
        {
            //return gc.Graph[^1].Count == 1 && gc.Graph[^1][0].RoomType == RoomType.Exit;
            
            var nodes = gc.Graph.SelectMany(x => x).ToList();
            var bossRoom = nodes.FindLast(x => x.RoomType == RoomType.Boss);
            return bossRoom != null && bossRoom.Children[0].RoomType == RoomType.Exit;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            var lowestLevel = gc.Graph[^1];
            var lastNode = lowestLevel[Random.Range(0, lowestLevel.Count)];

            var bossNode = gc.CreateChild(lastNode, RoomType.Boss);
            var exitNode = gc.CreateChild(bossNode, RoomType.Exit);
            
            bossNode.Lock();
            exitNode.Lock();
        }
    }
}