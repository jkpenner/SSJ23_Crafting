using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Strafe")]
    public class StrafeCard : AttachmentCard
    {
        [SerializeField] float strafeSpeedMod = 1f;

        public override CardType CardType => CardType.Move;
        public override AttachmentType AttachmentType => AttachmentType.Move;

        private StatMod mod;

        public override void OnCardEnable()
        {
            mod = StatMod.Flat(strafeSpeedMod);
            Owner.MoveSpeed.AddMod(mod);
        }

        public override void OnCardDisable()
        {
            Owner.MoveSpeed.RemoveMod(mod);
            mod = null;
        }

        public override void OnCardUpdate()
        {
            if (Owner.IsActionLocked)
            {
                return;
            }

            Owner.AllowMovement = true;
            Owner.MoveDirection = Owner.transform.right;
        }
    }
}