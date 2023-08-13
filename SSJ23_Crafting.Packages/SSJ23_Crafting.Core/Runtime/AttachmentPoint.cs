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

        public void Attach(AttachmentCard data)
        {

        } 

        public void Eject()
        {

        }
    }
}