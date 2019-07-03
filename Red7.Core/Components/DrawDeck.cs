using Red7.Core.Helpers;
using System.Collections.Generic;

namespace Red7.Core.Components
{
    public class DrawDeck
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        public Card DrawCard()
        {
            if (Cards.Count > 0)
                return Cards.RemoveAndGet(0);
            else
                throw new System.Exception("Cannot draw from an empty deck.");
        }

        public void Shuffle()
        {
            Cards.Shuffle();
        }
    }
}
