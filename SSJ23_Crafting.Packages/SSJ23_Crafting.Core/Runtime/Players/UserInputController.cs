using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class UserInputController : PlayerController
    {
        private GameEvents events;

        public override void OnEnable(Player player)
        {
            events = GameEvents.FindOrCreateInstance();
            events.UILaunchPressed.Register(OnLaunchPressed);
            events.UIUseCard.Register(OnUseCard);
        }

        public override void OnDisable(Player player)
        {
            if (events != null)
            {
                events.UILaunchPressed.Unregister(OnLaunchPressed);
                events.UIUseCard.Unregister(OnUseCard);
                events = null;
            }
        }

        private void OnLaunchPressed()
        {
            Debug.Log("Interacted With Launch Button");
        }

        private void OnUseCard(int index)
        {
            Debug.Log($"Interacted with card at index {index}");
        }
    }
}