using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Strafe")]
    public class StrafeCard : MoverCard
    {
        [SerializeField] float strafeSpeedMod = 1f;

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        private StatMod mod;

        public override void OnCardEnable(Robot robot, AttachmentPoint point)
        {
            mod = StatMod.Flat(strafeSpeedMod);
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
            Owner.MoveDirection = Owner.transform.right;
        }
    }
}