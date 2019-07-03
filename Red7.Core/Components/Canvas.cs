using System.Collections.Generic;
using System.Linq;

namespace Red7.Core.Components
{
    public class Canvas
    {
        public List<Card> Cards { get; set; } = new List<Card>() { new Card() { Color = Enums.Color.Red, Value = 0, Action = Enums.Action.None } };

        public void AddCardToCanvas(Card card)
        {
            Cards.Add(card);
        }

        public Card GetActiveCanvasCard()
        {
            return Cards.Last();
        }
    }
}
