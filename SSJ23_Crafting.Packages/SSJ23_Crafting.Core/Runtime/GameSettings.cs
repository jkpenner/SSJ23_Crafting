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
    }
}