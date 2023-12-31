using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Move")]
    public class MoveCard : AttachmentCard
    {
        [SerializeField] float moveSpeedMod = 1f;

        public override CardType CardType => CardType.Move;
        public override AttachmentType AttachmentType => AttachmentType.Move;

        private StatMod mod;

        public override void OnCardEnable()
        {
            mod = StatMod.Flat(moveSpeedMod);
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

            Owner.Motor.MoveVelocity = Owner.MoveSpeed.Value;
            Owner.Motor.Move(Owner.transform.forward);
        }
    }
}