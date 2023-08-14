using System.Collections.Generic;

namespace SSJ23_Crafting
{
    public class CardHand
    {
        public List<CardData> Cards { get; private set; }
        public int CardCount => Cards.Count;

        public CardHand()
        {
            Cards = new List<CardData>();
        }

        public void AddCard(CardData card)
        {
            Cards.Add(card);
        }

        public bool HasCard(CardData target)
        {
            foreach(var card in Cards)
            {
                if (card == target)
                {
                    return true;
                }
            }

            return false;
        }

        public bool RemoveCard(CardData target)
        {
            return Cards.Remove(target);
        }
    }
}