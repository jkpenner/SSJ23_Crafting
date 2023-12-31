using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Shaper")]
    public class ShaperCard : CardData
    {
        [SerializeField] Robot robotPrefab;

        public override CardType CardType => CardType.Shaper;
        public Robot RobotPrefab => robotPrefab;

        public override void OnCardUse(Player player)
        {
            base.OnCardUse(player);
            player.ChangeRobot(robotPrefab);
        }
    }
}