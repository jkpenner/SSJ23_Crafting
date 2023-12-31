using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIPlayerHand : MonoBehaviour
    {
        [SerializeField] PlayerId playerId;
        [SerializeField] UICard cardPrefab;
        [SerializeField] UIPlayerHandSlot[] slots;


        [Header("Sounds")]
        [SerializeField] RandomSound useCardSounds;
        [SerializeField] RandomSound drawCardSounds;
        [SerializeField] RandomSound discardCardSounds;

        private GameEvents events;
        private GameManager gameManager;

        private Coroutine drawCardRoutine;
        private Queue<CardData> drawCardQueue;

        private void Awake()
        {
            drawCardQueue = new Queue<CardData>();

            gameManager = GameManager.FindOrCreateInstance();
            events = GameEvents.FindOrCreateInstance();
        }

        private void OnEnable()
        {
            events.CardUsed.Register(OnCardUsed);
            events.CardDrawn.Register(OnCardDrawn);
            events.CardDiscarded.Register(OnCardDiscarded);
            events.GameOver.Register(OnGameOver);
        }


        private void OnDisable()
        {
            events.CardUsed.Unregister(OnCardUsed);
            events.CardDrawn.Unregister(OnCardDrawn);
            events.CardDiscarded.Unregister(OnCardDiscarded);
            events.GameOver.Unregister(OnGameOver);
        }

        private void OnGameOver()
        {
            foreach(var slot in slots)
            {
                if (slot.Card == null)
                {
                    continue; 
                }
                
                Destroy(slot.Card.gameObject);
                slot.SetCard(null);
            }
        }

        private void OnCardUsed(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            StartCoroutine(UseCardRoutine(args.card));
        }

        private void OnCardDrawn(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            if (drawCardRoutine != null)
            {
                drawCardQueue.Enqueue(args.card);
            }
            else
            {
                drawCardRoutine = StartCoroutine(DrawCardRoutine(args.card));
            }
        }

        private void OnCardDiscarded(CardEventArgs args)
        {
            if (args.playerId != playerId)
            {
                return;
            }

            StartCoroutine(DiscardCardRoutine(args.card));
        }

        private IEnumerator UseCardRoutine(CardData card)
        {
            Debug.Log("Use Card Routine Started");

            var uiCard = RemoveCard(card);
            if (uiCard is null)
            {
                Debug.Log("Used Card is not found");
                yield break;
            }

            // Handle Use Animation for Card

            useCardSounds?.PlayRandom();

            yield return MoveAllCardsLeft();

            Destroy(uiCard.gameObject);
        }

        private IEnumerator DiscardCardRoutine(CardData card)
        {
            Debug.Log("Discard Card Routine Started");

            var uiCard = RemoveCard(card);
            if (uiCard is null)
            {
                yield break;
            }

            // Handle Discard Animation for Card

            discardCardSounds?.PlayRandom();

            yield return MoveAllCardsLeft();

            Destroy(uiCard.gameObject);
        }

        private IEnumerator DrawCardRoutine(CardData card)
        {
            yield return MoveAllCardsLeft();

            // cardPrefab.gameObject.SetActive(false);
            var instance = Instantiate(cardPrefab);
            instance.Hide();
            instance.SetCard(card);

            var uiSlot = GetFirstEmptySlot();
            if (uiSlot is null)
            {
                Debug.LogWarning($"Failed to find open slot on card draw");
                yield break;
            }

            uiSlot.SetCard(instance);

            instance.transform.SetParent(uiSlot.transform.parent, false);

            yield return null;

            instance.transform.position = uiSlot.transform.position + Vector3.down * 40f;
            instance.transform.rotation = uiSlot.transform.rotation;
            instance.DragInput.SetOrigin(uiSlot.transform.position);
            instance.DragInput.RestoreToOrigin();

            instance.Used += OnCardUsed;
            instance.Discarded += OnCardDiscarded;

            instance.Show();

            drawCardSounds?.PlayRandom();

            if (drawCardQueue.Count > 0)
            {
                yield return new WaitForSeconds(0.2f);
                var nextCard = drawCardQueue.Dequeue();
                drawCardRoutine = StartCoroutine(DrawCardRoutine(nextCard));
            } else
            {
                drawCardRoutine = null;
            }
        }

        private void OnCardUsed(UICard card)
        {
            if (!gameManager.UseCard(playerId, card.Card))
            {
                card.DragInput.RestoreToOrigin();
            }
        }

        private void OnCardDiscarded(UICard card)
        {
            if (!gameManager.DiscardCard(playerId, card.Card))
            {
                card.DragInput.RestoreToOrigin();
            }
        }

        private IEnumerator MoveAllCardsLeft()
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
                    slots[i].Card.DragInput.RestoreToOrigin();
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
            yield return null;
        }

        private UIPlayerHandSlot GetFirstEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Card != null)
                {
                    continue;
                }

                return slots[i];
            }

            return null;
        }

        private UICard RemoveCard(CardData card)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Card is null || slots[i].Card.Card != card)
                {
                    continue;
                }

                var uiCard = slots[i].Card;
                slots[i].SetCard(null);
                return uiCard;
            }

            return null;
        }
    }
}