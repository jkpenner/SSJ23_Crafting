using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    /// <summary>
    /// Logger that will display GameEvents through Debug.Log when emitted.
    /// </summary>
    public class GameEventLogger : MonoBehaviour
    {
        private GameEvents events;

        private void OnEnable()
        {
            events = GameEvents.FindOrCreateInstance();
            events.ResourceChanged.Register(OnResourceChanged);
            events.CardUsed.Register(OnCardUsed);
            events.CardDiscarded.Register(OnCardDiscarded);
            events.CardDrawn.Register(OnCardDrawn);
            events.ShowDiscard.Register(OnShowDiscard);
            events.HideDiscard.Register(OnHideDiscard);
        }

        private void OnDisable()
        {
            events.ResourceChanged.Unregister(OnResourceChanged);
            events.CardUsed.Unregister(OnCardUsed);
            events.CardDiscarded.Unregister(OnCardDiscarded);
            events.CardDrawn.Unregister(OnCardDrawn);
            events.ShowDiscard.Unregister(OnShowDiscard);
            events.HideDiscard.Unregister(OnHideDiscard);
        }

        private void Log(string message)
        {
            Debug.Log($"[{nameof(GameEvent)}]: {message}");
        }

        private void OnResourceChanged(ResourceEventArgs args)
        {
            //Log($"Player {args.playerId} Resourced Changed By {args.amount} (Total {args.totalValue})");
        }

        private void OnCardUsed(CardEventArgs args)
        {
            Log($"Player {args.playerId} Used Card {args.card.DisplayName}");
        }

        private void OnCardDiscarded(CardEventArgs args)
        {
            Log($"Player {args.playerId} Discarded Card {args.card.DisplayName}");
        }

        private void OnCardDrawn(CardEventArgs args)
        {
            Log($"Player {args.playerId} Drawn Card {args.card.DisplayName}");
        }

        private void OnShowDiscard()
        {
            Log($"Discard Warnign Shown");
        }

        private void OnHideDiscard()
        {
            Log($"Discard Warnign Hidden");
        }
    }
}