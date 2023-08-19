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
        [SerializeField] TurnDirection direction = TurnDirection.Left;
        [SerializeField] float speed = 90f;
        [SerializeField] bool onlyWhileGrounded = true;

        public override AttachmentType AttachmentType => AttachmentType.MoverTurn;

        public override void OnCardUpdate(Robot robot, AttachmentPoint point)
        {
            var dir = direction == TurnDirection.Left ? -1f : 1f;
            robot.transform.Rotate(robot.transform.up * speed * dir * Time.deltaTime);
        }
    }
}