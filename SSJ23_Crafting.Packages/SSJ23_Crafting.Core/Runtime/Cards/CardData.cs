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


        public virtual bool IsUsable(Player player) {
            if (player.Resource < resourceCost)
            {
                return false;
            }

            return true;
        }

        public virtual void OnUse(Player player) {}
        public virtual void OnDiscard(Player player) {}
    }
}