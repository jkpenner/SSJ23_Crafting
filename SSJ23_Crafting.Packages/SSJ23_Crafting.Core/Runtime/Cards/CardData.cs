using UnityEngine;

namespace SSJ23_Crafting
{
    public abstract class CardData : ScriptableObject
    {
        [SerializeField] string displayName;
        [SerializeField] int resourceCost;

        public abstract CardType CardType { get; }
        public string DisplayName => displayName;
        public int ResourceCost => resourceCost;
    }
}