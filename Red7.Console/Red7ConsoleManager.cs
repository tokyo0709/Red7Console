using Colorful;
using Red7.Core;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
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
            var root = GetApplicationRoot();
            string newPath = Path.GetFullPath(Path.Combine(root, @"..\"));

            FigletFont font = FigletFont.Load($"{newPath}/Red7.Console/FigletFonts/standard.flf");
            Figlet figlet = new Figlet(font);

            Console.WriteLine(figlet.ToAscii("Red Seven"), Color.Red);
            Console.ReadLine();
        }
        
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

        public static void InitializeConsoleGame(int playerCount)
        {
            Console.Clear();

            SetBorderValues(playerCount);
            OtherPlayerBoardsMasked = true;

            Console.SetWindowSize(WidthValue, HeightValue);
            DrawBorder(Color.FloralWhite, 0, 0, WidthValue, BoardHeightValue); 
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
                Console.WriteLine("Palette:", Color.White);
                
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
            Console.CursorVisible = false;//Hide cursor
            Console.SetCursorPosition(left, top);
            Console.Write(s, color);
            Console.SetCursorPosition(currentLeft, currentTop);
            Console.CursorVisible = true;//Show cursor back
        }

        private static void DrawBorder(Color color, int originX, int originY, int width, int height)
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
                        else if (i == width - 1)
                            WriteAt(i, j, "┐", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + 1)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "┌", color);
                        else if (i == width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == width - 3)
                            WriteAt(i, j, "┐", color);
                        else if (i != originX + 1 && i != width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == height - 1)
                    {
                        if (i == originX)
                            WriteAt(i, j, "└", color);
                        else if (i == width - 1)
                            WriteAt(i, j, "┘", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == height - 2)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "└", color);
                        else if (i == width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == width - 3)
                            WriteAt(i, j, "┘", color);
                        else if (i != originX + 1 && i != width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else
                    {
                        if (i == originX || i == originX + 2)
                            WriteAt(i, j, "│", color);
                        else if (i == width - 1 || i == width - 3)
                            WriteAt(i, j, "│", color);
                    }
                }
            }
        }
    }
}
