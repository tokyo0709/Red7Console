using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Infrastructure;
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

        //public static List<ColorRule> GenerateColorRules()
        //{
        //    return new List<ColorRule>()
        //    {
        //        new ColorRule(Color.Red, "Highest Card"),
        //        new ColorRule(Color.Orange, "Most Of One Number"),
        //        new ColorRule(Color.Yellow, "Most Of One Color"),
        //        new ColorRule(Color.Green, "Most Even Cards"),
        //        new ColorRule(Color.Blue, "Most Different Colors"),
        //        new ColorRule(Color.Indigo, "Most Cards In A Row"),
        //        new ColorRule(Color.Violet, "Most Cards Below 4")
        //    };
        //}
    }
}
