using Red7.Core;
using Red7.Core.Helpers;
using System;
using System.Drawing;
using System.Linq;
using Console = Colorful.Console;

namespace Red7.ConsoleManager
{
    public static class Red7ConsoleManager
    {
        private static int HeightValue { get; set; } = 40;
        private static int BoardHeightValue { get; set; } = 20;
        private static int WidthValue { get; set; } = 200;
        private static int PlayerBoardWidth { get; set; } = 50;
        
        private static void SetBorderValues(int playerCount)
        {
            WidthValue = playerCount * 50;
        }

        public static void InitializeConsole(int playerCount)
        {
            SetBorderValues(playerCount);

            // Max 240/63
            DrawBorder(Color.FloralWhite);

            //Console.SetCursorPosition(3, 2);
            //Console.WriteLine("┌─┐", Color.Red);
            //Console.SetCursorPosition(3, 3);
            //Console.WriteLine("│1│", Color.Red);
            //Console.SetCursorPosition(3, 4);
            //Console.WriteLine("└─┘", Color.Red);
            //Console.ReadLine();
        }

        public static void DrawBoards(Red7Game red7Game)
        {
            var activePlayer = red7Game.Players.Where(x => x.ActivePlayer == true).First();

            for (int i = 0; i < red7Game.Players.Count; i++)
            {
                var player = red7Game.Players.ElementAt(i);

                Console.SetCursorPosition(3  +(i * PlayerBoardWidth), 2);
                Console.WriteLine($"{player.Name}", Color.LightCyan);

                Console.SetCursorPosition(3 + (i * PlayerBoardWidth), 4);
                Console.WriteLine("Palette:", Color.White);
                Console.SetCursorPosition(3 + (i * PlayerBoardWidth), 5);
                foreach (var item in player.Palette.Cards)
                {
                    Console.Write("|", Color.White);
                    Console.Write($"{item.Value}", item.Color.GetConsoleColor());
                    Console.Write("| ", Color.White);
                }

                Console.SetCursorPosition(3 + (i * PlayerBoardWidth), 7);
                Console.WriteLine("Hand:", Color.White);
                Console.SetCursorPosition(3 + (i * PlayerBoardWidth), 8);
                foreach (var item in player.Hand.Cards)
                {
                    Console.Write("|", Color.White);
                    if (player.Id != activePlayer.Id)
                    {
                        Console.Write("X", Color.White);
                    }
                    else
                    {
                        Console.Write($"{item.Value}", item.Color.GetConsoleColor());
                    }
                    Console.Write("| ", Color.White);
                }
            }

            //foreach (var player in red7Game.Players)
            //{
            //    Console.SetCursorPosition(3, 2);
            //    Console.WriteLine($"{player.Name}'s Board:\r\n", Color.LightCyan);

            //    Console.SetCursorPosition(3, 4);
            //    Console.WriteLine("Palette:", Color.White);
            //    Console.SetCursorPosition(3, 5);
            //    foreach (var item in player.Palette.Cards)
            //    {
            //        Console.Write("|", Color.White);
            //        Console.Write($"{item.Value}", item.Color.GetConsoleColor());
            //        Console.Write("| ", Color.White);
            //    }

            //    Console.SetCursorPosition(3, 7);
            //    Console.WriteLine("Hand:", Color.White);
            //    Console.SetCursorPosition(3, 8);
            //    foreach (var item in player.Hand.Cards)
            //    {
            //        Console.Write("|", Color.White);
            //        if (player.Id != activePlayer.Id)
            //        {
            //            Console.Write("X", Color.White);
            //        }
            //        else
            //        {
            //            Console.Write($"{item.Value}", item.Color.GetConsoleColor());
            //        }
            //        Console.Write("| ", Color.White);
            //    }
            //}

            Console.WriteLine("Canvas:", Color.LightGray);
            Console.Write("|", Color.White);
            Console.Write($"{red7Game.Canvas.GetActiveCanvasCard().Value}", red7Game.Canvas.GetActiveCanvasCard().Color.GetConsoleColor());
            Console.Write("|", Color.White);


            Console.WriteLine($"\r\n\r\n{red7Game.Players.Where(x => x.ActivePlayer == true).First().Name}'s turn.", Color.White);
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

        private static void DrawBorder(Color color)
        {
            Console.SetWindowSize(WidthValue, HeightValue);

            for (int i = 0; i < WidthValue; i++)
            {
                for (int j = 0; j < BoardHeightValue; j++)
                {
                    if (j == 0)
                    {
                        if (i == 0)
                            WriteAt(i, j, "┌", color);
                        else if (i == WidthValue - 1)
                            WriteAt(i, j, "┐", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == 1)
                    {
                        if (i == 0)
                            WriteAt(i, j, "│", color);
                        else if (i == 1)
                            WriteAt(i, j, "┌", color);
                        else if (i == WidthValue - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == WidthValue - 2)
                            WriteAt(i, j, "┐", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == BoardHeightValue - 1)
                    {
                        if (i == 0)
                            WriteAt(i, j, "└", color);
                        else if (i == WidthValue - 1)
                            WriteAt(i, j, "┘", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == BoardHeightValue - 2)
                    {
                        if (i == 0)
                            WriteAt(i, j, "│", color);
                        else if (i == 1)
                            WriteAt(i, j, "└", color);
                        else if (i == WidthValue - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == WidthValue - 2)
                            WriteAt(i, j, "┘", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else
                    {
                        if (i == 0 || i == 1)
                            WriteAt(i, j, "│", color);
                        else if (i == WidthValue - 1 || i == WidthValue - 2)
                            WriteAt(i, j, "│", color);
                    }
                }
            }
        }
    }
}
