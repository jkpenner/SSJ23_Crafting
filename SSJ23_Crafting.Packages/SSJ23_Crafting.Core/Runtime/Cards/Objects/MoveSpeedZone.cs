using UnityEngine;

namespace SSJ23_Crafting
{
    public class MoveSpeedZone : MonoBehaviour
    {
        [SerializeField] float moveSpeedModifier = 0f;

        private StatMod moveSpeedMod;

        private void Awake()
        {
            moveSpeedMod = StatMod.Flat(moveSpeedModifier);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Robot>(out var robot))
            {
                return;
            }

            robot.MoveSpeed.AddMod(moveSpeedMod);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<Robot>(out var robot))
            {
                return;
            }

            robot.MoveSpeed.RemoveMod(moveSpeedMod);
        }
    }
}