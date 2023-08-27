using System;
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

        private float switchCounter = 0f;

        public override void OnCardEnable()
        {
            mod = StatMod.Flat(strafeSpeedMod);
            Owner.MoveSpeed.AddMod(mod);
            Owner.Motor.OnHitWall += OnHitWall;
        }

        public override void OnCardDisable()
        {
            Owner.Motor.OnHitWall -= OnHitWall;
            Owner.MoveSpeed.RemoveMod(mod);
            mod = null;
        }

        private void OnHitWall(Vector3 vector)
        {
            if (Owner.IsActionLocked)
            {
                return;
            }

            // Change directions when we hit a wall;
            var newMod = StatMod.Flat(-mod.Amount);
            Owner.MoveSpeed.RemoveMod(mod);
            
            mod = newMod;
            Owner.MoveSpeed.AddMod(mod);
        }

        public override void OnCardUpdate()
        {
            if (Owner.IsActionLocked)
            {
                return;
            }

            Owner.Motor.MoveVelocity = Owner.MoveSpeed.Value;
            Owner.Motor.Move(Owner.transform.right);
        }
    }
}