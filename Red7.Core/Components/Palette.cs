using System.Collections.Generic;

namespace Red7.Core.Components
{
    public class Palette
    {
        public int PlayerId { get; set; }
        public List<Card> Cards { get; set; }

        public Palette(int playerId)
        {
            PlayerId = playerId;
            Cards = new List<Card>();
        }

        public void AddCardToPalette(Card card)
        {
            Cards.Add(card);
        }
    }
}
