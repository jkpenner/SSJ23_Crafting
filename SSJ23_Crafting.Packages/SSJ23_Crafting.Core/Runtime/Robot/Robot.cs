using UnityEngine;

namespace SSJ23_Crafting
{
    public enum RobotState
    {
        Build,
        Launch,
        Battle,
        Dead,
    }

    public class Robot : MonoBehaviour
    {
        [SerializeField] AttachmentPoint[] attachmentPoints;
        [SerializeField] float launchDuration = 1f;

        public PlayerId PlayerId { get; private set; }
        public RobotState State { get; private set; }

        private Vector3 launchStart;
        private Vector3 launchMiddle;
        private Vector3 launchTarget;
        private float launchPercent;

        public bool IsValidAttachment(AttachmentType attachmentType)
        {
            foreach (var attachmentPoint in attachmentPoints)
            {
                if (attachmentPoint.AttachmentType == attachmentType)
                {
                    return true;
                }
            }

            return false;
        }

        public void Attach(AttachmentCard attachment)
        {
            foreach (var attachmentPoint in attachmentPoints)
            {
                if (attachmentPoint.AttachmentType == attachment.AttachmentType)
                {
                    attachmentPoint.Attach(this, attachment);
                }
            }
        }

        public void Launch(Vector3 target)
        {
            launchStart = transform.position;
            launchTarget = target;

            launchMiddle = new Vector3(
                (launchTarget.x - launchStart.x) / 2f + launchStart.x,
                Mathf.Max(launchStart.y, launchTarget.y) + 4f,
                (launchTarget.z - launchStart.z) / 2f + launchStart.z
            );

            launchPercent = 0f;

            SetState(RobotState.Launch);
            transform.SetParent(null);
        }

        private void Update()
        {
            if (State == RobotState.Launch)
            {
                launchPercent += 1f / GameSettings.LaunchDuration * Time.deltaTime;
                if (launchPercent >= 1f)
                {
                    launchPercent = 1f;
                }

                var a = Vector3.Lerp(launchStart, launchMiddle, launchPercent);
                var b = Vector3.Lerp(launchMiddle, launchTarget, launchPercent);
                transform.position = Vector3.Lerp(a, b, launchPercent);

                if (launchPercent >= 1f)
                {
                    SetState(RobotState.Battle);
                }
            }
            else if (State == RobotState.Battle)
            {
                foreach (var attachmentPoint in attachmentPoints)
                {
                    attachmentPoint.Update(this);
                }
            }
        }

        private void SetState(RobotState state)
        {
            if (State != state)
            {
                // Leaving a state
                switch (State)
                {
                    case RobotState.Battle:
                        foreach (var attachmentPoint in attachmentPoints)
                        {
                            attachmentPoint.Disable(this);
                        }
                        break;
                }

                State = state;

                // Entering a state
                switch (State)
                {
                    case RobotState.Battle:
                        foreach (var attachmentPoint in attachmentPoints)
                        {
                            attachmentPoint.Enable(this);
                        }
                        break;
                }
            }
        }
    }
}