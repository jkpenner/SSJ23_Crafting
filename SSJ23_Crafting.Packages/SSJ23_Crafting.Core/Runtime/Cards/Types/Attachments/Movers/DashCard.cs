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

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            if (robot.IsActionLocked && robot.ActionLockOwner != this)
            {
                return;
            }

            counter += Time.deltaTime;

            if (isDashing)
            {
                var movement = robot.transform.forward * speed * Time.deltaTime;
                robot.Rigidbody.MovePosition(robot.transform.position + movement);

                if (counter >= duration)
                {
                    isDashing = false;
                    counter = 0f;
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