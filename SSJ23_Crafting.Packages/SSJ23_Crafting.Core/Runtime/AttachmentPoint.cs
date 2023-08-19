using UnityEngine;

namespace SSJ23_Crafting
{
    [System.Flags]
    public enum AttachmentType
    {
        None = 0x0,
        Damager = 0x2,
        Defender = 0x4,

        MoverJump = 0x08,
        MoverMove = 0x10,
        MoverTurn = 0x20,
    }

    public class AttachmentPoint : MonoBehaviour
    {
        [SerializeField] AttachmentType attachmentType;

        public AttachmentType AttachmentType => attachmentType;
        public AttachmentCard AttachmentData { get; private set; }

        public bool HasAttachment => AttachmentData != null;

        public void Attach(Robot robot, AttachmentCard data)
        {
            if (data is null)
            {
                throw new System.NullReferenceException("Can not attach null Attachment. Use Eject to remove.");
            }

            if (HasAttachment)
            {
                Detach(robot);
            }

            AttachmentData = data;
            AttachmentData.OnCardAttach(robot, this);
        }

        public void Detach(Robot robot)
        {
            if (!HasAttachment)
            {
                return;
            }

            AttachmentData.OnCardDetach(robot, this);
            AttachmentData = null;
        }

        public void Enable(Robot robot)
        {
            if (AttachmentData != null)
            {
                AttachmentData.OnCardEnable(robot, this);
            }
        }

        public void Disable(Robot robot)
        {
            if (AttachmentData != null)
            {
                AttachmentData.OnCardDisable(robot, this);
            }
        }

        public void UpdateAttachment(Robot robot)
        {
            if (AttachmentData != null)
            {
                AttachmentData.OnCardUpdate(robot, this);
            }
        }
    }
}