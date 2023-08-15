using System;
using UnityEngine;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    public class UILaunchPressed : MonoBehaviour
    {
        [SerializeField] PlayerId playerId;
        [SerializeField] Button button;
        private GameManager gameManager;

        private void OnEnable()
        {
            gameManager = GameManager.FindOrCreateInstance();

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

            gameManager = null;
        }

        private void OnClicked()
        {
            gameManager.LaunchRobot(playerId);
        }
    }
}