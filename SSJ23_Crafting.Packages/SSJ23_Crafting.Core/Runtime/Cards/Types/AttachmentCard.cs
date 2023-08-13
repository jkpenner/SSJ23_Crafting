namespace SSJ23_Crafting
{
    public abstract class AttachmentCard : CardData
    {
        public abstract AttachmentType AttachmentType { get; }

        /// <summary>
        /// Invoked when the Attachment Card is attached to a Robot.
        /// </summary>
        public virtual void OnAttach(Robot robot) {}

        /// <summary>
        /// Invoked each frame the robot is active.
        /// </summary>
        public virtual void OnUpdate(Robot robot) {}

        /// <summary>
        /// Invoked when the Attachment Card is detached from a robot.
        /// </summary>
        public virtual void OnDetach(Robot robot) {}
    }
}