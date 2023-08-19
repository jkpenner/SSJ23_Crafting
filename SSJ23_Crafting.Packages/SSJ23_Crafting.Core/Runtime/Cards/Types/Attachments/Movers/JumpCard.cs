using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Jump")]
    public class JumpCard : MoverCard
    {
        [SerializeField] float height = 3f;
        [SerializeField] float duration = 1f;
        [SerializeField] float interval = 5f;

        private float counter = 0f;
        private bool isJumping = false;
        private float startHeight = 0f;

        public override AttachmentType AttachmentType => AttachmentType.MoverJump;

        public override void OnUpdate(Robot robot, AttachmentPoint point)
        {
            counter += Time.deltaTime;
            if (isJumping)
            {
                var t = Mathf.Clamp01(counter / duration);

                var a = Mathf.Lerp(startHeight, startHeight + height, t);
                var b = Mathf.Lerp(startHeight + height, startHeight, t);
                
                robot.transform.position = new Vector3(
                    robot.transform.position.x,
                    Mathf.Lerp(a, b, t),
                    robot.transform.position.z
                );

                if (counter >= duration)
                {
                    isJumping = false;
                    counter = 0f;
                }
            }
            else
            {
                if (counter >= interval)
                {
                    startHeight = robot.transform.position.y;
                    isJumping = true;
                    counter = 0f;
                }
            }
        }
    }
}