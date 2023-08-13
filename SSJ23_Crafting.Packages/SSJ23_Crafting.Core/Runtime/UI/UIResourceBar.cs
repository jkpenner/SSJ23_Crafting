using System;
using UnityEngine;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    public class UIResourceBar : MonoBehaviour
    {
        [SerializeField] PlayerId playerId;
        [SerializeField] RectTransform fill = null;
        [SerializeField] Image fillImage = null;

        private RectTransform rectTransform = null;
        public float FillPercent { get; private set; }

        private GameEvents events;

        private void Awake()
        {
            // Ensure the fill settings are correct
            fill.anchorMin = new Vector2(0f, 0f);
            fill.anchorMax = new Vector2(1f, 1f);
            fill.pivot = new Vector2(0.0f, 0.5f);

            rectTransform = GetComponent<RectTransform>();
            events = GameEvents.FindOrCreateInstance();
            SetPercent(0.0f);
        }

        private void OnEnable()
        {
            events.ResourceChanged.Register(OnResourceChanged);
        }

        private void OnDisable()
        {
            events.ResourceChanged.Unregister(OnResourceChanged);
        }

        private void OnResourceChanged(ResourceEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            SetPercent(args.totalValue / GameSettings.MaxResource);
        }

        public void SetColor(Color color)
        {
            fillImage.color = color;
        }

        public void SetPercent(float percent)
        {
            FillPercent = Mathf.Clamp01(percent);

            fill.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                rectTransform.rect.width * FillPercent
            );
        }
    }
}