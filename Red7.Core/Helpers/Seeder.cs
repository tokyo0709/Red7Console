using Red7.Core.Components;
using Red7.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Red7.Core.Helpers
{
    public static class Seeder
    {
        public static List<Card> GenerateCards()
        {
            var cards = new List<Card>();

            for (int i = 1; i < 8; i++)
            {
                foreach (Color color in (Color[])Enum.GetValues(typeof(Color)))
                {
                    var card = new Card { Color = color, Value = i };
                    switch (i)
                    {
                        case 1:
                            card.Action = Enums.Action.AttackRemove;
                            break;
                        case 3:
                            card.Action = Enums.Action.Draw;
                            break;
                        case 5:
                            card.Action = Enums.Action.DoublePlay;
                            break;
                        case 7:
                            card.Action = Enums.Action.Remove;
                            break;
                        default:
                            card.Action = Enums.Action.None;
                            break;
                    }
                    cards.Add(card);
                }
            }

            return cards;
        }
    }
}
