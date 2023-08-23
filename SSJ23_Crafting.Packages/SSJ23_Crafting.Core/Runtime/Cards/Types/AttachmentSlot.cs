using UnityEngine;

namespace SSJ23_Crafting
{
    [System.Serializable]
    public class AttachmentSlot
    {
        [SerializeField] AttachmentSlotType type;

        public AttachmentSlotType Type => type;
        public AttachmentCard Card { get; set; }

        public bool HasAttachment => Card != null;

        public bool IsValidAttachment(AttachmentCard card)
        {
            return AttachmentUtility.IsValid(type, card.AttachmentType);
        }
    }
}