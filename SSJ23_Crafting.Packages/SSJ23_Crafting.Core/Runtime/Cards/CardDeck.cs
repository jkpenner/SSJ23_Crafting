using System.Collections.Generic;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class CardDeck
    {
        private CardDeckData source;
        private List<CardData> cards = new List<CardData>();

        public void SetSource(CardDeckData data)
        {
            source = data;
        }

        public int CardCount => cards.Count;
        public bool IsEmpty => CardCount == 0;

        public void Clear()
        {
            cards.Clear();
        }

        /// <summary>
        /// Populates the deck with the current source data.
        /// </summary>
        public void Populate()
        {
            if (source is null)
            {
                Debug.LogError("Unable to populate from a null source");
                return;
            }

            Clear();
            foreach(var card in source.Cards)
            {
                cards.Add(ScriptableObject.Instantiate(card));
            }
        }

        /// <summary>
        /// Populates the card deck from the given asset
        /// </summary>
        public void Populate(CardDeckData data)
        {
            SetSource(data);
            Clear();
            foreach(var card in data.Cards)
            {
                cards.Add(ScriptableObject.Instantiate(card));
            }
        }

        /// <summary>
        /// Randomly orders the cards within the deck.
        /// </summary>
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, cards.Count);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        /// <summary>
        /// Attempts to draw a card from the deck. Will return false
        /// if no more cards are in the deck.
        /// </summary>
        public bool TryDraw(out CardData outCard)
        {
            if (IsEmpty)
            {
                outCard = null;
                return false;
            }

            outCard = cards[0];
            cards.RemoveAt(0);
            return true;
        }
    }
}