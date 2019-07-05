﻿using Colorful;
using Red7.ConsoleManager.Helpers;
using Red7.ConsoleManager.Models;
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
        private static SetupMenu SetupMenu { get; } = new SetupMenu
        {
            MenuOptions = new List<MenuOption>()
            {
                new MenuOption(Option.AddPlayers, true),
                new MenuOption(Option.DiscardDrawRule, false),
                new MenuOption(Option.ScoringRule, false),
                new MenuOption(Option.ActionRule, false),
                new MenuOption(Option.GameStart, false)
            }
        };
        
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

            WriteMenu(red7Game);
            ConsoleHelper.DrawBorder(Color.White, 5, 32, 57, 6);
            
            while (!Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    var index = SetupMenu.MenuOptions.FindIndex(y => y.Active);
                    SetupMenu.MenuOptions.ElementAt(index).Active = false;
                    if (index == 0)
                        SetupMenu.MenuOptions.ElementAt(SetupMenu.MenuOptions.Count - 1).Active = true;
                    else
                        SetupMenu.MenuOptions.ElementAt(index - 1).Active = true;

                    WriteMenu(red7Game);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    var index = SetupMenu.MenuOptions.FindIndex(y => y.Active);
                    SetupMenu.MenuOptions.ElementAt(index).Active = false;
                    if (SetupMenu.MenuOptions.Count - 1 > index)
                        SetupMenu.MenuOptions.ElementAt(index + 1).Active = true;
                    else
                        SetupMenu.MenuOptions.ElementAt(0).Active = true;

                    WriteMenu(red7Game);
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
        }

        private static void WriteMenu(Red7Game game)
        {
            WriteIntroduction(58, 5, 9, Color.White);
            WriteAddPlayer(58, 5, 12, SetupMenu.MenuOptions.Where(x => x.Option == Option.AddPlayers).First().Active ? Color.Yellow : Color.White);
            WriteDiscardDrawRule(58, 5, 14, SetupMenu.MenuOptions.Where(x => x.Option == Option.DiscardDrawRule).First().Active ? Color.Yellow : Color.White, game);
            WriteScoringRule(58, 5, 18, SetupMenu.MenuOptions.Where(x => x.Option == Option.ScoringRule).First().Active ? Color.Yellow : Color.White, game);
            WriteActionRule(58, 5, 21, SetupMenu.MenuOptions.Where(x => x.Option == Option.ActionRule).First().Active ? Color.Yellow : Color.White, game);
            WriteGameStart(58, 5, 24, SetupMenu.MenuOptions.Where(x => x.Option == Option.GameStart).First().Active ? Color.Yellow : Color.White, game);
        }

        private static void WriteIntroduction(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "Welcome to the game Red 7. Before we get started begin by adding some players and setting up your game settings.", color);
        }

        private static void WriteAddPlayer(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "1) Add Players", color);
        }

        private static void WriteDiscardDrawRule(int width, int left, int top, Color color, Red7Game game)
        {
            if (!game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.DiscardDraw).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(width, left, top, "2) Enable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", color);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 14, "2) Disable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", color);
        }

        private static void WriteScoringRule(int width, int left, int top, Color color, Red7Game game)
        {
            if (!game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(58, 5, 18, "3) Enable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", color);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 18, "3) Disable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", color);
        }

        private static void WriteActionRule(int width, int left, int top, Color color, Red7Game game)
        {
            if (!game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(58, 5, 21, "4) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", color);
            else
                ConsoleHelper.WriteWordWrapAt(58, 5, 21, "4) Enable Action Rule (Forced actions when playing odd numbered cards to your palette)", color);
        }

        private static void WriteGameStart(int width, int left, int top, Color color, Red7Game game)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "5) Start Game", color);
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
