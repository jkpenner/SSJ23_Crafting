using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Dash")]
    public class DashCard : MoverCard
    {
        [SerializeField] float speed = 1f;
        [SerializeField] float duration = 2f;
        [SerializeField] float interval = 5f;

        private float counter = 0f;
        private bool isDashing = false;

        private StatMod moveMod;

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        public override void OnCardEnable(Robot robot, AttachmentPoint point)
        {
            moveMod = StatMod.Flat(speed);
            Owner.MoveSpeed.AddMod(moveMod);
        }

        public override void OnCardDisable(Robot robot, AttachmentPoint point)
        {
            Owner.MoveSpeed.RemoveMod(moveMod);
            moveMod = null;
        }

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            if (robot.IsActionLocked && robot.ActionLockOwner != this)
            {
                return;
            }

            counter += Time.deltaTime;

            if (isDashing)
            {
                Owner.AllowMovement = true;
                Owner.MoveDirection = Owner.transform.forward;

                if (counter >= duration)
                {
                    isDashing = false;
                    counter = 0f;
                    Owner.AllowMovement = false;
                    robot.ReleaseActionLock(this);
                }
            }
            else if (counter >= interval && robot.SetActionLock(this))
            {
                isDashing = true;
                counter = 0f;
            }            
        }
    }
}