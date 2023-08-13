using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Card Deck")]
    public class CardDeckData : ScriptableObject
    {
        [SerializeField] CardData[] cards;

        public CardData[] Cards => cards;
    }
}