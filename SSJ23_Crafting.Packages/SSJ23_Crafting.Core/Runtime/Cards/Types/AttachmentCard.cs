namespace SSJ23_Crafting
{
    public abstract class AttachmentCard : CardData
    {
        public abstract AttachmentType AttachmentType { get; }

        public override bool IsUsable(Player player)
        {
            if (!base.IsUsable(player) || player.Robot == null)
            {
                return false;
            }

            return player.Robot.IsValidAttachment(AttachmentType);
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
        public virtual void OnCardAttach(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked each frame the robot is active.
        /// </summary>
        public virtual void OnCardUpdate(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when the Attachment Card is detached from a robot.
        /// </summary>
        public virtual void OnCardDetach(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when robot is enabled during battle.
        /// </summary>
        public virtual void OnCardEnable(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when robot is disabled after battle.
        /// </summary>
        public virtual void OnCardDisable(Robot robot, AttachmentPoint point) {}
    }
}