using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(UIDragInput))]
    public class UICard : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] TMPro.TMP_Text nameText;
        [SerializeField] TMPro.TMP_Text typeText;
        [SerializeField] TMPro.TMP_Text costText;
        [SerializeField] Transform visualParent;

        private GameEvents events;
        public UIDragInput DragInput { get; private set; }

        public Action<UICard> Used;
        public Action<UICard> Discarded;

        public CardData Card { get; private set; }

        private void Awake()
        {
            events = GameEvents.FindOrCreateInstance();
            DragInput = GetComponent<UIDragInput>();
        }

        private void OnEnable()
        {
            DragInput.DragCanceled += OnDragCanceled;
            DragInput.DropZoneChanged += OnDropZoneChanged;
        }

        private void OnDisable()
        {
            DragInput.DragCanceled -= OnDragCanceled;
            DragInput.DropZoneChanged -= OnDropZoneChanged;
        }

        private void OnDragCanceled()
        {
            if (DragInput.Zone == UIDragInput.DropZone.ActionOne)
            {
                Used?.Invoke(this);
            }
            else if (DragInput.Zone == UIDragInput.DropZone.ActionTwo)
            {
                Discarded?.Invoke(this);
            }

            events.HideDiscard.Emit();
        }

        private void OnDropZoneChanged()
        {
            if (DragInput.Zone == UIDragInput.DropZone.ActionTwo)
            {
                events.ShowDiscard.Emit();
            }
            else
            {
                events.HideDiscard.Emit();
            }
        }

        public void SetCard(CardData card)
        {
            Card = card;
            if (Card != null)
            {
                nameText.text = Card.DisplayName;
                costText.text = "0"; // Todo: Replace with energy cost
                typeText.text = Card.CardType switch
                {
                    CardType.Shaper => "Shaper",
                    CardType.Mover => "Mover",
                    CardType.Damager => "Damager",
                    CardType.Defender => "Defender",
                    CardType.Ejector => "Ejector",
                    _ => "Unknown"
                };

                if (visualParent != null && Card.VisualPrefab != null)
                {
                    for (int i = 0; i < visualParent.childCount; i++)
                    {
                        Destroy(visualParent.GetChild(i).gameObject);
                    }
                    var instance = GameObject.Instantiate(Card.VisualPrefab);
                    instance.transform.SetParent(visualParent, true);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = Vector3.one;
                    instance.layer = LayerMask.NameToLayer("UI");
                }
            }
        }
    }
}