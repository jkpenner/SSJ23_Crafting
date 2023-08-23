namespace SSJ23_Crafting
{
    [System.Flags]
    public enum AttachmentType
    {
        None = 0x0,
        
        Damage = 0x2,
        Defend = 0x4,

        Jump = 0x08,
        Move = 0x10,
        Turn = 0x20,
    }
}