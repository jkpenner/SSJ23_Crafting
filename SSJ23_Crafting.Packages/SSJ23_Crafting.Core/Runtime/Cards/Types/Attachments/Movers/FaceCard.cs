using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Face")]
    public class FaceCard : AttachmentCard
    {
        [SerializeField] float speed = 90f;
        [SerializeField] float distance = 30f;
        [SerializeField] bool onlyWhileGrounded = true;

        public override CardType CardType => CardType.Turn;
        public override AttachmentType AttachmentType => AttachmentType.Turn;

        private StatMod turnMod;

        public override void OnCardEnable()
        {
            turnMod = StatMod.Flat(speed);
            Owner.TurnSpeed.AddMod(turnMod);
        }

        public override void OnCardDisable()
        {
            Owner.TurnSpeed.RemoveMod(turnMod);
            turnMod = null;
        }

        public override void OnCardUpdate()
        {
            if (!Owner.IsGrounded && onlyWhileGrounded)
            {
                return;
            }

            Robot target = null;
            float targetDistance = float.MaxValue;

            foreach(var collider in Physics.OverlapSphere(Owner.transform.position, targetDistance))
            {
                var other = collider.GetComponent<Robot>();
                if (other == null)
                {
                    continue;
                }

                if (other.PlayerId == Owner.PlayerId)
                {
                    continue;
                }

                var distance = Vector3.Distance(Owner.transform.position, other.transform.position);
                if (distance >= targetDistance)
                {
                    continue;
                }

                target = other;
                targetDistance = distance;
            }

            if (target == null)
            {
                return;
            }

            var direction = target.transform.position - Owner.transform.position;
            direction.y = 0;
            direction.Normalize();

            Owner.AllowRotation = true;
            Owner.TurnDirection = direction;
            Owner.RotationMode = RotationMode.Face;
        }
    }
}