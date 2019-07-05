using Colorful;
using Red7.ConsoleManager.Helpers;
using Red7.Core;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace Red7.ConsoleManager
{
    public static class Red7ConsoleManager
    {
        private static int HeightValue { get; set; } = 40;
        private static int BoardHeightValue { get; set; } = 24;
        private static int WidthValue { get; set; } = 200;
        private static int PlayerBoardWidth { get; set; } = 50;
        private static int CardWidth { get; set; } = 3;
        private static bool OtherPlayerBoardsMasked { get; set; } = false;
        
        private static void SetBorderValues(int playerCount)
        {
            WidthValue = playerCount * 50;
        }

        public static void InitializeConsoleSetup(Red7Game red7Game)
        {
            Console.SetBufferSize(120, 40);
            Console.SetWindowSize(120, 40);
            Console.CursorVisible = false;

            var root = FileHelper.GetApplicationRoot();
            string newPath = Path.GetFullPath(Path.Combine(root, @"..\"));

            FigletFont font = FigletFont.Load($"{newPath}/Red7.Console/FigletFonts/standard.flf");
            Figlet figlet = new Figlet(font);

            Console.Write(figlet.ToAscii("Red Seven"), Color.Red);
            ConsoleHelper.DrawBorder(Color.Red, 1, 6, 65, 34, true);
            ConsoleHelper.DrawBorder(Color.White, 69, 0, 50, 40, true);

            //Console.SetCursorPosition(4, 9);
            ConsoleHelper.WriteWordWrapAt(58, 5, 9, "Welcome to the game Red 7. Before we get started begin by adding some players and setting up your game settings.", Color.White);
            ConsoleHelper.WriteWordWrapAt(58, 5, 12, "1) Add Players", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.DiscardDraw).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(58, 5, 14, "2) Enable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", Color.White);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 14, "2) Disable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(58, 5, 18, "3) Enable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", Color.White);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 18, "3) Disable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(58, 5, 21, "4) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", Color.White);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 21, "4) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", Color.White);

            ConsoleHelper.DrawBorder(Color.White, 5, 32, 57, 6);

            Console.ReadLine();
        }

        public static void InitializeConsoleGame(int playerCount)
        {
            Console.Clear();

            SetBorderValues(playerCount);
            OtherPlayerBoardsMasked = true;

            Console.SetBufferSize(WidthValue, HeightValue);
            Console.SetWindowSize(WidthValue, HeightValue);
            ConsoleHelper.DrawBorder(Color.FloralWhite, 0, 0, WidthValue, BoardHeightValue, true); 
        }

        public static void DrawBoards(Red7Game red7Game)
        {
            var activePlayer = red7Game.Players.Where(x => x.ActivePlayer == true).First();

            // Loop through players and draw individual player boards
            for (int i = 0; i < red7Game.Players.Count; i++)
            {
                var player = red7Game.Players.ElementAt(i);

                ConsoleHelper.DrawBoxedWord(4 + (i * PlayerBoardWidth), 2, player.Name, player.ActivePlayer ? Color.Yellow : Color.White);

                // Draw Palette cards
                Console.SetCursorPosition(4 + (i * PlayerBoardWidth), 6);
                Console.Write("Palette:", Color.White);

                foreach (var card in player.Palette.Cards)
                {
                    // Palette boxed cards
                    for (int j = 0; j <= 2; j++)
                    {
                        if (j == 0) // Box tops
                        {
                            Console.SetCursorPosition(4 + (i * PlayerBoardWidth), 7);
                            Console.Write("┌─┐", card.Color.GetConsoleColor());
                        }
                        else if (j == 1) // Box contents
                        {
                            Console.SetCursorPosition(4 + (i * PlayerBoardWidth), 8);
                            Console.Write($"│{card.Value}│", card.Color.GetConsoleColor());
                        }
                        else if (j == 2) // Box bottoms
                        {
                            Console.SetCursorPosition(4 + (i * PlayerBoardWidth), 9);
                            Console.Write("└─┘", card.Color.GetConsoleColor());
                        }
                    }
                }

                // Draw hand cards
                Console.SetCursorPosition(4 + (i * PlayerBoardWidth), 11);
                Console.WriteLine("Hand:", Color.White);

                for (int k = 0; k < player.Hand.Cards.Count; k++)
                {
                    var card = player.Hand.Cards.ElementAt(k);
                    var cardColor = OtherPlayerBoardsMasked && !player.ActivePlayer ? Color.White : card.Color.GetConsoleColor();
                    var cardValue = OtherPlayerBoardsMasked && !player.ActivePlayer ? "│X│" : $"│{card.Value}│";

                    // Hand boxed cards
                    for (int j = 0; j <= 2; j++)
                    {
                        if (j == 0) // Box tops
                        {
                            Console.SetCursorPosition(4 + (k * CardWidth) + (i * PlayerBoardWidth), 12);
                            Console.Write("┌─┐", cardColor);
                        }
                        else if (j == 1) // Box contents
                        {
                            Console.SetCursorPosition(4 + (k * CardWidth) + (i * PlayerBoardWidth), 13);
                            Console.Write(cardValue, cardColor);
                        }
                        else if (j == 2) // Box bottoms
                        {
                            Console.SetCursorPosition(4 + (k * CardWidth) + (i * PlayerBoardWidth), 14);
                            Console.Write("└─┘", cardColor);
                        }
                    }
                }
            } // End Player Board Loop

            ConsoleHelper.DrawBoxedWord(4, 16, "Draw Deck", Color.White);
            ConsoleHelper.DrawBoxedWord(16, 16, $"{red7Game.Deck.Cards.Count}", Color.White);

            var activeCanvasCard = red7Game.Canvas.GetActiveCanvasCard();
            ConsoleHelper.DrawBoxedWord(4, 19, "Canvas", Color.White);
            ConsoleHelper.DrawBoxedWord(14, 19, $"{activeCanvasCard.Value}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));
            ConsoleHelper.DrawBoxedWord(19, 19, $"{ColorRules.GetRuleByColor(activeCanvasCard.Color).RuleDescription}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));

            Console.CursorVisible = false;

            Console.ReadLine();
        }
    }
}
