using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIPlayerHand : MonoBehaviour
    {
        [SerializeField] UICard[] cardSlots;

        private void Awake()
        {
            for (int i = 0; i < cardSlots.Length; i++)
            {
                cardSlots[i].CardIndex = i;
            }
        }
    }
}