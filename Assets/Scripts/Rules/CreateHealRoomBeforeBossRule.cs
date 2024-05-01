using System.Linq;
using UnityEngine;

namespace GraphRewriting.Rules
{
    [CreateAssetMenu(fileName = "CreateHealRoomBeforeBossRule", menuName = "Rules/CreateHealRoomBeforeBossRule")]
    public class CreateHealRoomBeforeBossRule : Rule
    {
        public override bool IsEnforced(GraphCreator gc)
        {
            var nodes = gc.Graph.SelectMany(x => x).ToList();
            var bossRoom = nodes.FindLast(x => x.RoomType == RoomType.Boss);

            if (!bossRoom) return true;
            return bossRoom.Parent.RoomType == RoomType.Healing && bossRoom.Parent.IsLocked;
        }

        public override void EnforceRule(GraphCreator gc)
        {
            var nodes = gc.Graph.SelectMany(x => x).ToList();
            var bossRoom = nodes.FindLast(x => x.RoomType == RoomType.Boss);
            
            bossRoom.Parent.SetType(RoomType.Healing);
            bossRoom.Parent.Lock();
        }
    }
}