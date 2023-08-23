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

        public override AttachmentType AttachmentType => AttachmentType.Turn;

        private StatMod turnMod;

        public override void OnCardEnable()
        {
            turnMod = StatMod.Flat(speed);
            Owner.TurnSpeed.AddMod(turnMod);
        }

        public override void OnCardDisable()
        {
            Owner.TurnSpeed.RemoveMod(turnMod);
            turnMod = null;
        }

        public override void OnCardUpdate()
        {
            if (!Owner.IsGrounded && onlyWhileGrounded)
            {
                return;
            }

            Owner.AllowRotation = true;
            Owner.RotationMode = RotationMode.Turn;
        }
    }
}