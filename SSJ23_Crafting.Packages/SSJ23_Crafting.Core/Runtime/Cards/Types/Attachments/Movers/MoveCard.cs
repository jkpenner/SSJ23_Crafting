using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Movers/Move Card")]
    public class MoveCard : MoverCard
    {
        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        public override void OnUpdate(Robot robot)
        {

        }
    }
}