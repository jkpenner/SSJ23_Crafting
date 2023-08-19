using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Move")]
    public class MoveCard : MoverCard
    {
        [SerializeField] float speed = 1f;

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            var movement = robot.transform.forward * speed * Time.deltaTime;
            robot.Rigidbody.MovePosition(robot.transform.position + movement);
        }
    }
}