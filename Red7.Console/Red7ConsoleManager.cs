using Colorful;
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
        
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static void InitializeConsoleSetup(Red7Game red7Game)
        {
            Console.SetBufferSize(120, 40);
            Console.SetWindowSize(120, 40);
            Console.CursorVisible = false;

            var root = GetApplicationRoot();
            string newPath = Path.GetFullPath(Path.Combine(root, @"..\"));

            FigletFont font = FigletFont.Load($"{newPath}/Red7.Console/FigletFonts/standard.flf");
            Figlet figlet = new Figlet(font);

            Console.Write(figlet.ToAscii("Red Seven"), Color.Red);
            DrawBorder(Color.Red, 1, 6, 65, 34, true);
            DrawBorder(Color.White, 69, 0, 50, 40, true);

            //Console.SetCursorPosition(4, 9);
            WriteWordWrapAt(58, 5, 9, "Welcome to the game Red 7. Before we get started begin by adding some players and setting up your game settings.", Color.White);
            WriteWordWrapAt(58, 5, 12, "1) Add Players", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.DiscardDraw).First().Enabled)
                WriteWordWrapAt(58, 5, 14, "2) Enable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", Color.White);
            else
                WriteWordWrapAt(58, 5, 14, "2) Disable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First().Enabled)
                WriteWordWrapAt(58, 5, 18, "3) Enable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", Color.White);
            else
                WriteWordWrapAt(58, 5, 18, "3) Disable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", Color.White);

            if (!red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First().Enabled)
                WriteWordWrapAt(58, 5, 21, "3) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", Color.White);
            else
                WriteWordWrapAt(58, 5, 21, "3) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", Color.White);

            DrawBorder(Color.White, 5, 32, 57, 6);

            Console.ReadLine();
        }

        public static void WriteWordWrapAt(int width, int left, int top, string paragraph, Color color)
        {
            string[] lines = paragraph
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string process = lines[i];
                List<String> wrapped = new List<string>();

                while (process.Length > width)
                {
                    int wrapAt = process.LastIndexOf(' ', Math.Min(width - 1, process.Length));
                    if (wrapAt <= 0) break;

                    wrapped.Add(process.Substring(0, wrapAt));
                    process = process.Remove(0, wrapAt + 1);
                }

                foreach (string wrap in wrapped)
                {
                    WriteAt(left, top, wrap, color);
                    top++;
                }

                WriteAt(left, top, process, color);
            }
        }

        public static void InitializeConsoleGame(int playerCount)
        {
            Console.Clear();

            SetBorderValues(playerCount);
            OtherPlayerBoardsMasked = true;

            Console.SetBufferSize(WidthValue, HeightValue);
            Console.SetWindowSize(WidthValue, HeightValue);
            DrawBorder(Color.FloralWhite, 0, 0, WidthValue, BoardHeightValue, true); 
        }

        private static void DrawBoxedWord(int left, int top, string word, Color color)
        {
            // Player Name
            for (int l = 0; l <= 2; l++)
            {
                if (l == 0) // Box tops
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write("┌", color);

                    foreach (var chr in word)
                    {
                        Console.Write("─", color);
                    }

                    Console.Write("┐", color);
                }
                else if (l == 1) // Box contents
                {
                    Console.SetCursorPosition(left, top + 1);
                    Console.Write($"│{word}│", color);
                }
                else if (l == 2) // Box bottoms
                {
                    Console.SetCursorPosition(left, top + 2);

                    Console.Write("└", color);

                    foreach (var chr in word)
                    {
                        Console.Write("─", color);
                    }

                    Console.Write("┘", color);
                }
            }
        }

        public static void DrawBoards(Red7Game red7Game)
        {
            var activePlayer = red7Game.Players.Where(x => x.ActivePlayer == true).First();

            // Loop through players and draw individual player boards
            for (int i = 0; i < red7Game.Players.Count; i++)
            {
                var player = red7Game.Players.ElementAt(i);

                DrawBoxedWord(4 + (i * PlayerBoardWidth), 2, player.Name, player.ActivePlayer ? Color.Yellow : Color.White);
                
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

            DrawBoxedWord(4, 16, "Draw Deck", Color.White);
            DrawBoxedWord(16, 16, $"{red7Game.Deck.Cards.Count}", Color.White);

            var activeCanvasCard = red7Game.Canvas.GetActiveCanvasCard();
            DrawBoxedWord(4, 19, "Canvas", Color.White);
            DrawBoxedWord(14, 19, $"{activeCanvasCard.Value}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));
            DrawBoxedWord(19, 19, $"{ColorRules.GetRuleByColor(activeCanvasCard.Color).RuleDescription}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));

            Console.CursorVisible = false;

            Console.ReadLine();
        }

        private static void WriteAt(int left, int top, string s, Color color)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            Console.SetCursorPosition(left, top);
            Console.Write(s, color);
            Console.SetCursorPosition(currentLeft, currentTop);
        }

        private static void DrawBorder(Color color, int originX, int originY, int width, int height, bool doubleBorder = false)
        {
            // i = Column
            for (int i = originX; i < width + originX; i++)
            {
                // j = Row
                for (int j = originY; j < height + originY; j++)
                {
                    if (j == originY)
                    {
                        if (i == originX)
                            WriteAt(i, j, "┌", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "┐", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + 1 && doubleBorder)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "┌", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 3)
                            WriteAt(i, j, "┐", color);
                        else if (i != originX + 1 && i != originX + width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + height - 1)
                    {
                        if (i == originX)
                            WriteAt(i, j, "└", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "┘", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + height - 2 && doubleBorder)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "└", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 3)
                            WriteAt(i, j, "┘", color);
                        else if (i != originX + 1 && i != originX + width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else
                    {
                        if (i == originX || (i == originX + 2 && doubleBorder))
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 1 || (i == originX + width - 3 && doubleBorder))
                            WriteAt(i, j, "│", color);
                    }
                }
            }
        }
    }
}
