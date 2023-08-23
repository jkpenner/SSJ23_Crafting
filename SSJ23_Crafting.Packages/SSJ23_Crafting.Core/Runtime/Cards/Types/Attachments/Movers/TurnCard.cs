using UnityEngine;

namespace SSJ23_Crafting
{
    public enum TurnDirection
    {
        Left, Right
    }

    [CreateAssetMenu(menuName = "Cards/Turn")]
    public class TurnCard : MoverCard
    {
        [SerializeField] float speed = 90f;
        [SerializeField] bool onlyWhileGrounded = true;

        public override AttachmentType AttachmentType => AttachmentType.MoverTurn;

        private StatMod turnMod;

        public override void OnCardEnable(Robot robot, AttachmentPoint point)
        {
            turnMod = StatMod.Flat(speed);
            Owner.TurnSpeed.AddMod(turnMod);
        }

        public override void OnCardDisable(Robot robot, AttachmentPoint point)
        {
            Owner.TurnSpeed.RemoveMod(turnMod);
            turnMod = null;
        }

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            if (!robot.IsGrounded && onlyWhileGrounded)
            {
                return;
            }

            Owner.AllowRotation = true;
            Owner.RotationMode = RotationMode.Turn;
        }
    }
}