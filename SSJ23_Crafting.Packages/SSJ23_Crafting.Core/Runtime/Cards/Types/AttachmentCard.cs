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

        public override void OnUse(Player player)
        {
            base.OnUse(player);
            if (player.Robot != null)
            {
                player.Robot.Attach(this);
            }
        }

        /// <summary>
        /// Invoked when the Attachment Card is attached to a Robot.
        /// </summary>
        public virtual void OnAttach(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked each frame the robot is active.
        /// </summary>
        public virtual void OnUpdate(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when the Attachment Card is detached from a robot.
        /// </summary>
        public virtual void OnDetach(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when robot is enabled during battle.
        /// </summary>
        public virtual void OnEnable(Robot robot, AttachmentPoint point) {}

        /// <summary>
        /// Invoked when robot is disabled after battle.
        /// </summary>
        public virtual void OnDisable(Robot robot, AttachmentPoint point) {}
    }
}