using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Jump")]
    public class JumpCard : AttachmentCard
    {
        [SerializeField] float jumpSpeedMod = 3f;
        [SerializeField] float interval = 5f;

        private float counter = 0f;
        private bool isJumping = false;

        private StatMod jumpMod;

        public override CardType CardType => CardType.Jump;
        public override AttachmentType AttachmentType => AttachmentType.Jump;

        public override void OnCardEnable()
        {
            jumpMod = StatMod.Flat(jumpSpeedMod);
            Owner.JumpSpeed.AddMod(jumpMod);
            Owner.Motor.OnGrounded += OnLandedOnGround;
        }

        public override void OnCardDisable()
        {
            Owner.Motor.OnGrounded -= OnLandedOnGround;
            Owner.JumpSpeed.RemoveMod(jumpMod);
            jumpMod = null;
        }

        private void OnLandedOnGround()
        {
            isJumping = false;
            counter = 0f;
            Owner.ReleaseActionLock(this);
        }

        public override void OnCardUpdate()
        {
            if (Owner.IsActionLocked && Owner.ActionLockOwner != this)
            {
                return;
            }

            counter += Time.deltaTime;

            if (!isJumping && counter >= interval && Owner.SetActionLock(this))
            {
                isJumping = true;
                Owner.Motor.Jump();
            }
        }
    }
}