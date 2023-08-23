using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Jump")]
    public class JumpCard : MoverCard
    {
        [SerializeField] float jumpSpeedMod = 3f;
        [SerializeField] float interval = 5f;

        private float counter = 0f;
        private bool isJumping = false;

        private StatMod jumpMod;

        public override AttachmentType AttachmentType => AttachmentType.MoverJump;

        public override void OnCardEnable(Robot robot, AttachmentPoint point)
        {
            jumpMod = StatMod.Flat(jumpSpeedMod);
            Owner.JumpSpeed.AddMod(jumpMod);
            Owner.LandedOnGround += OnLandedOnGround;
        }

        public override void OnCardDisable(Robot robot, AttachmentPoint point)
        {
            Owner.LandedOnGround -= OnLandedOnGround;
            Owner.JumpSpeed.RemoveMod(jumpMod);
            jumpMod = null;
        }

        private void OnLandedOnGround()
        {
            isJumping = false;
            counter = 0f;
            Owner.ReleaseActionLock(this);
        }

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            if (Owner.IsActionLocked && Owner.ActionLockOwner != this)
            {
                return;
            }

            counter += Time.deltaTime;

            if (!isJumping && counter >= interval && Owner.SetActionLock(this))
            {
                isJumping = true;
                Owner.Jump();
            }
        }
    }
}