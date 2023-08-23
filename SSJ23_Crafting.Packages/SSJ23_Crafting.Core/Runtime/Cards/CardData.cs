using UnityEngine;

namespace SSJ23_Crafting
{
    public abstract class CardData : ScriptableObject
    {
        [SerializeField] GameObject visualPrefab;
        [SerializeField] string displayName;

        public abstract CardType CardType { get; }
        public string DisplayName => displayName;
        public GameObject VisualPrefab => visualPrefab;

        public virtual bool IsUsable(Player player) {
            return true;
        }

        public virtual void OnCardUse(Player player) {}
        public virtual void OnCardDiscard(Player player) {}
    }
}