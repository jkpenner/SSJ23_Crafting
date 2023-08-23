using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Move")]
    public class MoveCard : MoverCard
    {
        [SerializeField] float moveSpeedMod = 1f;

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        private StatMod mod;

        public override void OnCardEnable(Robot robot, AttachmentPoint point)
        {
            mod = StatMod.Flat(moveSpeedMod);
            Owner.MoveSpeed.AddMod(mod);
        }

        public override void OnCardDisable(Robot robot, AttachmentPoint point)
        {
            Owner.MoveSpeed.RemoveMod(mod);
            mod = null;
        }

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            if (robot.IsActionLocked)
            {
                return;
            }

            Owner.AllowMovement = true;
            Owner.MoveDirection = Owner.transform.forward;
        }
    }
}