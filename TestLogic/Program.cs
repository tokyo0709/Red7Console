using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
using System.Collections.Generic;

namespace TestLogic
{
    class Program
    {
        static void Main(string[] args)
        {
            var activePalette = new Palette(1)
            {
                Cards = new List<Card>
                {
                    new Card
                    {
                        Color = Color.Orange,
                        Value = 2
                    },
                    new Card
                    {
                        Color = Color.Blue,
                        Value = 4
                    },
                    new Card
                    {
                        Color = Color.Yellow,
                        Value = 3
                    },
                    new Card
                    {
                        Color = Color.Red,
                        Value = 2
                    },
                    new Card
                    {
                        Color = Color.Yellow,
                        Value = 4
                    },
                }
            };

            var opponentPalettes = new List<Palette>();
            opponentPalettes.Add(new Palette(2)
            {
                Cards = new List<Card>
                {
                    new Card
                    {
                        Color = Color.Green,
                        Value = 4
                    },
                    new Card
                    {
                        Color = Color.Violet,
                        Value = 2
                    },
                    new Card
                    {
                        Color = Color.Yellow,
                        Value = 2
                    },
                    new Card
                    {
                        Color = Color.Red,
                        Value = 5
                    },
                    new Card
                    {
                        Color = Color.Yellow,
                        Value = 6
                    },
                }
            });
            opponentPalettes.Add(new Palette(3)
            {
                Cards = new List<Card>
                {
                    new Card
                    {
                        Color = Color.Green,
                        Value = 5
                    },
                    new Card
                    {
                        Color = Color.Violet,
                        Value = 5
                    },
                    new Card
                    {
                        Color = Color.Yellow,
                        Value = 1
                    },
                    new Card
                    {
                        Color = Color.Red,
                        Value = 7
                    },
                    new Card
                    {
                        Color = Color.Green,
                        Value = 6
                    },
                }
            });

            GameLogic.IsWinningPalette(Color.Orange, activePalette, opponentPalettes, ColorRules.GetRuleByColor(Color.Orange));

            Console.ReadLine();
        }
    }
}
