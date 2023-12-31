namespace SSJ23_Crafting
{
    public static class AttachmentUtility
    {
        /// <summary>
        /// Checks if an attachment type is valid for a given attachement slot type.
        /// </summary>
        public static bool IsValid(AttachmentSlotType slotType, AttachmentType type)
        {
            return slotType switch
            {
                AttachmentSlotType.Damage => type.HasFlag(AttachmentType.Damage),
                AttachmentSlotType.Defend => type.HasFlag(AttachmentType.Defend),
                AttachmentSlotType.Jump => type.HasFlag(AttachmentType.Jump),
                AttachmentSlotType.Move => type.HasFlag(AttachmentType.Move),
                AttachmentSlotType.Turn => type.HasFlag(AttachmentType.Turn),
                _ => false
            };
        }

        public static CardType GetCardType(AttachmentSlotType slotType)
        {
            return slotType switch
            {
                AttachmentSlotType.Damage => CardType.Damager,
                AttachmentSlotType.Defend => CardType.Defender,
                AttachmentSlotType.Jump => CardType.Jump,
                AttachmentSlotType.Move => CardType.Move,
                AttachmentSlotType.Turn => CardType.Turn,
                _ => throw new System.InvalidOperationException()
            };
        }
    }
}