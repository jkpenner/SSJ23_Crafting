using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Game Settings")]
    public class GameSettings : ScriptableObject
    {
        public float ResourceRegenRate = 1.0f;
        public float MaxResource = 10f;
        public int ScoreToWin = 5;
        public int MaxHandSize = 6;

        public bool RepopulateEmptyDeck = true;
        public float LaunchDistance = 7f;
        public float LaunchDuration = 0.5f;

        public float GameRoundDuration = 60f;



        public Sprite ShaperCardIcon;
        public Sprite MoveCardIcon;
        public Sprite TurnCardIcon;
        public Sprite JumpCardIcon;
        public Sprite DamageCardIcon;
        public Sprite DefendCardIcon;

        public Sprite GetCardIcon(CardType type)
        {
            return type switch {
                CardType.Shaper => ShaperCardIcon,
                CardType.Move => MoveCardIcon,
                CardType.Jump => JumpCardIcon,
                CardType.Turn => TurnCardIcon,
                CardType.Damager => DamageCardIcon,
                CardType.Defender => DefendCardIcon,
                _ => null
            };
        }


        [Header("AI")]
        public float DelayBetweenAction = 2.5f;
        public float PercentChanceToLaunch = 0.25f;
    }
}