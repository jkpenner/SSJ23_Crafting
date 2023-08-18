using UnityEngine;

namespace SSJ23_Crafting
{
    public enum RobotState
    {
        Build,
        Launch,
        Battle,
        Dead,
    }

    public class Robot : MonoBehaviour
    {
        [SerializeField] AttachmentPoint[] attachmentPoints;

        public PlayerId PlayerId { get; private set; }
        public RobotState State { get; private set; }
    }
}