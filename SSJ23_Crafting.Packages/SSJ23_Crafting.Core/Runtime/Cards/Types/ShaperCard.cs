using UnityEngine;

namespace SSJ23_Crafting
{
    public class ShaperCard : CardData
    {
        [SerializeField] Robot robotPrefab;

        public override CardType CardType => CardType.Shaper;
        public Robot RobotPrefab => robotPrefab;
    }
}