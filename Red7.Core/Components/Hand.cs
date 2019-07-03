using System.Collections.Generic;

namespace Red7.Core.Components
{
    public class Hand
    {
        public int PlayerId { get; set; }
        public List<Card> Cards { get; set; }

        public Hand(int playerId)
        {
            PlayerId = playerId;
            Cards = new List<Card>();
        }

        public void AddCardToHand(Card card)
        {
            Cards.Add(card);
        }
    }
}
