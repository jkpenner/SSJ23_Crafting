using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    public class UIBuildElement : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TMP_Text displayName;

        public CardData Card { get; private set; }
        public AttachmentSlot Slot { get; private set; }

        public void SetCardType(CardType cardType)
        {
            icon.sprite = GameManager.FindOrCreateInstance()
                .Settings.GetCardIcon(cardType);
        }

        public void SetCardName(string name)
        {
            displayName.SetText(name);
        }

        public void SetSlot(AttachmentSlot slot)
        {
            Slot = slot;
        }

        public void UpdateValues()
        {
            if (Slot == null)
            {
                icon.sprite = null;
                displayName.SetText("Invalid");
                return;
            }

            SetCardType(Slot.CardType);
            if (Slot.HasAttachment)
            {
                SetCardName(Slot.Card.DisplayName);
            }
            else
            {
                SetCardName("Empty");
            }
        }
    }
}