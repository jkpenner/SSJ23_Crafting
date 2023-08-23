namespace SSJ23_Crafting
{
    public abstract class AttachmentCard : CardData
    {
        public Robot Owner { get; set; }
        public abstract AttachmentType AttachmentType { get; }

        public override bool IsUsable(Player player)
        {
            if (!base.IsUsable(player) || player.Robot == null)
            {
                return false;
            }

            return player.Robot.IsValidAttachment(this);
        }

        public override void OnCardUse(Player player)
        {
            base.OnCardUse(player);
            if (player.Robot != null)
            {
                player.Robot.Attach(this);
            }
        }

        /// <summary>
        /// Invoked when the Attachment Card is attached to a Robot.
        /// </summary>
        public virtual void OnCardAttach() {}

        /// <summary>
        /// Invoked each frame the robot is active.
        /// </summary>
        public virtual void OnCardUpdate() {}

        /// <summary>
        /// Invoked when the Attachment Card is detached from a robot.
        /// </summary>
        public virtual void OnCardDetach() {}

        /// <summary>
        /// Invoked when robot is enabled during battle.
        /// </summary>
        public virtual void OnCardEnable() {}

        /// <summary>
        /// Invoked when robot is disabled after battle.
        /// </summary>
        public virtual void OnCardDisable() {}
    }
}