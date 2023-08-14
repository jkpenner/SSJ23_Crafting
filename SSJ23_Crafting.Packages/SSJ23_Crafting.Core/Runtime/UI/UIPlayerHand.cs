using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIPlayerHand : MonoBehaviour
    {
        [SerializeField] PlayerId playerId;
        [SerializeField] UICard cardPrefab;
        [SerializeField] UIPlayerHandSlot[] slots;

        private GameEvents events;

        private void Awake()
        {
            events = GameEvents.FindOrCreateInstance();
        }

        private void OnEnable()
        {
            events.CardUsed.Register(OnCardUsed);
            events.CardDrawn.Register(OnCardDrawn);
            events.CardDiscarded.Register(OnCardDiscarded);
        }

        private void OnDisable()
        {
            events.CardUsed.Unregister(OnCardUsed);
            events.CardDrawn.Unregister(OnCardDrawn);
            events.CardDiscarded.Unregister(OnCardDiscarded);
        }

        private void OnCardUsed(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            RemoveCard(args.card);
            MoveCardsLeft();
        }

        private void OnCardDrawn(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            cardPrefab.gameObject.SetActive(false);
            var card = Instantiate(cardPrefab);
            card.SetCard(args.card);

            MoveCardsLeft();
            AddNewCard(card);
        }

        private void OnCardDiscarded(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            RemoveCard(args.card);
            MoveCardsLeft();
        }

        private void MoveCardsLeft()
        {
            for (int i = 0; i < slots.Length - 1; i++)
            {
                if (slots[i].Card != null)
                {
                    continue;
                }

                for (int j = i + 1; j < slots.Length; j++)
                {
                    if (slots[i].Card is null)
                    {
                        continue;
                    }

                    slots[i].SetCard(slots[j].Card);
                    slots[j].SetCard(null);
                }

                // Card was assign move to next open slot
                if (slots[i] != null)
                {
                    continue;
                }

                // No cards found to fill space exit early
                break;
            }
        }

        private void AddNewCard(UICard card)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Card != null)
                {
                    continue;
                }

                slots[i].SetCard(card);
            }
        }

        private void RemoveCard(CardData card)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Card != card)
                {
                    continue;
                }

                var uiCard = slots[i].Card;
                slots[i].SetCard(null);
                Destroy(uiCard.gameObject);
            }
        }
    }
}