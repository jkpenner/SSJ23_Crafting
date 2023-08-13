using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Movers/Jump Card")]
    public class JumpCard : MoverCard
    {
        public override AttachmentType AttachmentType => AttachmentType.MoverJump;

        public override void OnUpdate(Robot robot)
        {
            
        }
    }
}