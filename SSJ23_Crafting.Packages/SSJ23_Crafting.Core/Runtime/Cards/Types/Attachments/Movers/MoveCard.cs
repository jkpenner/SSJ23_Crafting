using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Move")]
    public class MoveCard : MoverCard
    {
        [SerializeField] float speed = 1f;

        public override AttachmentType AttachmentType => AttachmentType.MoverMove;

        public override void OnUpdate(Robot robot, AttachmentPoint point)
        {
            robot.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        }
    }
}