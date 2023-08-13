using UnityEngine;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    
    public class UICard : MonoBehaviour
    {
        [SerializeField] Button button;

        private GameEvents events;

        public int CardIndex { get; set; }

        private void OnEnable()
        {
            events = GameEvents.FindOrCreateInstance();

            if (button != null)
            {
                button.onClick.AddListener(OnClicked);
            }
            else
            {
                Debug.LogError($"[{GetType().Name}]: Button is not assigned");
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }

            events = null;
        }

        private void OnClicked()
        {
            events.UIUseCard.Emit(CardIndex);
        }
    }
}