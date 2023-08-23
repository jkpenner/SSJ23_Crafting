using UnityEngine;

namespace SSJ23_Crafting
{
    public enum AttachmentPointType
    {
        Left,
        Right,
    }

    public class AttachmentPoint : MonoBehaviour
    {
        [SerializeField] AttachmentPointType attachmentPointType;

        public AttachmentPointType Type => attachmentPointType;
    }
}