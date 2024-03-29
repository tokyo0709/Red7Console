﻿using Colorful;
using Red7.ConsoleManager.Helpers;
using Red7.ConsoleManager.Models;
using Red7.Core;
using Red7.Core.Components;
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
        private static int HeightValue { get; set; } = 47;
        private static int BoardHeightValue { get; set; } = 24;
        private static int MinWidthValue { get; set; } = 120;
        private static int WidthValue { get; set; } = 200;
        private static int PlayerBoardWidth { get; set; } = 40;
        private static int CardWidth { get; set; } = 3;
        private static bool OtherPlayerBoardsMasked { get; set; } = false;
        private static SetupMenu SetupMenu { get; } = new SetupMenu
        {
            MenuOptions = new List<SetupMenuOption>()
            {
                new SetupMenuOption(SetupOption.AddPlayers, true),
                new SetupMenuOption(SetupOption.DiscardDrawRule, false),
                new SetupMenuOption(SetupOption.ScoringRule, false),
                new SetupMenuOption(SetupOption.ActionRule, false),
                new SetupMenuOption(SetupOption.GameStart, false)
            }
        };
        private static ActionMenu ActionMenu { get; } = new ActionMenu
        {
            MenuOptions = new List<ActionMenuOption>()
            {
                new ActionMenuOption(ActionOption.PlayToPalette, true),
                new ActionMenuOption(ActionOption.PlayToCanvas, false),
                new ActionMenuOption(ActionOption.PlayToPaletteThenCanvas, false),
                new ActionMenuOption(ActionOption.Fold, false)
            }
        };

        public static void InitializeConsoleSetup(Red7Game red7Game)
        {
            // Initialize Setup Console Sizing
            Console.SetWindowSize(120, 40);
            Console.SetBufferSize(120, 40);
            Console.CursorVisible = false;

            WriteFigletTitle();
            ConsoleHelper.DrawBorder(Color.Red, 1, 6, 65, 34, true); // Menu Border
            ConsoleHelper.DrawBorder(Color.White, 69, 0, 51, 40, true); // Setup Option Display Border

            WriteSetupMenu(red7Game); // Setup Menu
            WriteConsoleSectionBorder(Color.White); // Input Output Setup Section

            WriteSettings(red7Game); // Current Configuration Settings

            // Console Menu Loop
            while (!Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    var index = SetupMenu.MenuOptions.FindIndex(y => y.Active);
                    SetupMenu.MenuOptions.ElementAt(index).Active = false;
                    if (index == 0)
                        SetupMenu.MenuOptions.ElementAt(SetupMenu.MenuOptions.Count - 1).Active = true;
                    else
                        SetupMenu.MenuOptions.ElementAt(index - 1).Active = true;

                    WriteSetupMenu(red7Game);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    var index = SetupMenu.MenuOptions.FindIndex(y => y.Active);
                    SetupMenu.MenuOptions.ElementAt(index).Active = false;
                    if (SetupMenu.MenuOptions.Count - 1 > index)
                        SetupMenu.MenuOptions.ElementAt(index + 1).Active = true;
                    else
                        SetupMenu.MenuOptions.ElementAt(0).Active = true;

                    WriteSetupMenu(red7Game);
                }
                else if (keyInfo.Key == ConsoleKey.Enter && SetupMenu.MenuOptions.Where(x => x.Active).First().Option == SetupOption.GameStart)
                {
                    red7Game.BeginGame();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = SetupMenu.MenuOptions.Where(x => x.Active).First();

                    switch (selectedOption.Option)
                    {
                        case SetupOption.AddPlayers:
                            AddPlayerPrompt(red7Game);
                            break;
                        case SetupOption.DiscardDrawRule:
                            ToggleDiscardDrawRule(red7Game);
                            break;
                        case SetupOption.ScoringRule:
                            ToggleScoringRule(red7Game);
                            break;
                        case SetupOption.ActionRule:
                            ToggleActionRule(red7Game);
                            break;
                        default:
                            break;
                    }
                }

                // Empty the buffer if necessary
                if (Console.KeyAvailable)
                    Console.ReadKey(false);
            }
        }

        public static void InitializeConsoleGame(Red7Game game)
        {
            // Clear the setup menu
            Console.Clear();

            SetBorderValues(game.Players.Count); // Dynamic game border values for player count
            OtherPlayerBoardsMasked = true; // Visibility on off player turn boards

            Console.SetWindowSize(WidthValue, HeightValue);
            Console.SetBufferSize(WidthValue, HeightValue);

            ConsoleHelper.DrawBorder(Color.FloralWhite, 0, 0, WidthValue, BoardHeightValue, false); // Player Board Section Border
            DrawBoards(game);
            ConsoleHelper.DrawBorder(Color.White, 0, BoardHeightValue, WidthValue, HeightValue - BoardHeightValue, false); // Action Section Border
            WriteActionMenu();
            WriteActionHistorySectionBorder(Color.White); // Action Output Section
            WriteActionInputSectionBorder(Color.White); // Action Input Section

            DrawRuleHelper(); // ROYGBIV Color Reference Helper

            // Action Menu Loop
            while (!Console.KeyAvailable)
            {
                var activePlayer = game.Players.Where(x => x.Active == true).First();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    var index = ActionMenu.MenuOptions.FindIndex(y => y.Active);
                    ActionMenu.MenuOptions.ElementAt(index).Active = false;
                    if (index == 0)
                        ActionMenu.MenuOptions.ElementAt(ActionMenu.MenuOptions.Count - 1).Active = true;
                    else
                        ActionMenu.MenuOptions.ElementAt(index - 1).Active = true;

                    WriteActionMenu();
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    var index = ActionMenu.MenuOptions.FindIndex(y => y.Active);
                    ActionMenu.MenuOptions.ElementAt(index).Active = false;
                    if (ActionMenu.MenuOptions.Count - 1 > index)
                        ActionMenu.MenuOptions.ElementAt(index + 1).Active = true;
                    else
                        ActionMenu.MenuOptions.ElementAt(0).Active = true;

                    WriteActionMenu();
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = ActionMenu.MenuOptions.Where(x => x.Active).First();

                    switch (selectedOption.Option)
                    {
                        case ActionOption.PlayToPalette:
                            SelectCardToPlayToPalette(game, activePlayer);
                            break;
                        case ActionOption.PlayToCanvas:
                            SelectCardToPlayToCanvas(game, activePlayer);
                            break;
                        case ActionOption.PlayToPaletteThenCanvas:

                            if (activePlayer.Hand.Cards.Count < 2)
                            {
                                EraseActionInputSection();
                                ConsoleHelper.WriteWordWrapAt(78, 24, 42, "Must have at least 2 cards (Press any key to continue)", Color.Yellow);

                                Console.ReadKey(true);
                                EraseActionInputSection();
                                continue;
                            }

                            SelectCardToPlayToPaletteThenCanvas(game, activePlayer);

                            break;
                        case ActionOption.Fold:
                            break;
                        default:
                            break;
                    }
                }

                // Empty the buffer if necessary
                if (Console.KeyAvailable)
                    Console.ReadKey(false);
            }
        }

        public static void DrawRuleHelper()
        {
            var ruleDescriptionLength = ColorRules.GetAllColorRules().OrderByDescending(s => s.RuleDescription.Length).First().RuleDescription.Length;
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 1, "R", ColorConverter.GetConsoleColor(Core.Enums.Color.Red));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 2, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Red));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 1, ColorRules.Red.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Red));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 4, "O", ColorConverter.GetConsoleColor(Core.Enums.Color.Orange));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 5, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Orange));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 4, ColorRules.Orange.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Orange));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 7, "Y", ColorConverter.GetConsoleColor(Core.Enums.Color.Yellow));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 8, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Yellow));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 7, ColorRules.Yellow.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Yellow));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 10, "G", ColorConverter.GetConsoleColor(Core.Enums.Color.Green));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 11, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Green));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 10, ColorRules.Green.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Green));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 13, "B", ColorConverter.GetConsoleColor(Core.Enums.Color.Blue));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 14, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Blue));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 13, ColorRules.Blue.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Blue));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 16, "I", ColorConverter.GetConsoleColor(Core.Enums.Color.Indigo));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 17, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Indigo));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 16, ColorRules.Indigo.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Indigo));

            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 8, BoardHeightValue + 19, "V", ColorConverter.GetConsoleColor(Core.Enums.Color.Violet));
            ConsoleHelper.WriteAt(WidthValue - ruleDescriptionLength - 5, BoardHeightValue + 20, "-", ColorConverter.GetConsoleColor(Core.Enums.Color.Violet));
            ConsoleHelper.DrawBoxedWord(WidthValue - ruleDescriptionLength - 4, BoardHeightValue + 19, ColorRules.Violet.RuleDescription, ColorConverter.GetConsoleColor(Core.Enums.Color.Violet));

        }

        public static void DrawBoards(Red7Game red7Game)
        {
            var activePlayer = red7Game.Players.Where(x => x.Active == true).First();

            DrawPlayerBoards(red7Game);

            ConsoleHelper.DrawBoxedWord(4, 16, "Draw Deck", Color.White);
            ConsoleHelper.DrawBoxedWord(16, 16, $"{red7Game.Deck.Cards.Count}", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 41, 19, "R", ColorConverter.GetConsoleColor(Core.Enums.Color.Red));
            ConsoleHelper.WriteAt(WidthValue - 37, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 35, 19, "O", ColorConverter.GetConsoleColor(Core.Enums.Color.Orange));
            ConsoleHelper.WriteAt(WidthValue - 31, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 29, 19, "Y", ColorConverter.GetConsoleColor(Core.Enums.Color.Yellow));
            ConsoleHelper.WriteAt(WidthValue - 25, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 23, 19, "G", ColorConverter.GetConsoleColor(Core.Enums.Color.Green));
            ConsoleHelper.WriteAt(WidthValue - 19, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 17, 19, "B", ColorConverter.GetConsoleColor(Core.Enums.Color.Blue));
            ConsoleHelper.WriteAt(WidthValue - 13, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 11, 19, "I", ColorConverter.GetConsoleColor(Core.Enums.Color.Indigo));
            ConsoleHelper.WriteAt(WidthValue - 7, 20, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(WidthValue - 5, 19, "V", ColorConverter.GetConsoleColor(Core.Enums.Color.Violet));

            DrawCanvas(red7Game);

            Console.CursorVisible = false;
        }

        private static void DrawCanvas(Red7Game red7Game)
        {
            var activeCanvasCard = red7Game.Canvas.GetActiveCanvasCard();
            ConsoleHelper.DrawBoxedWord(4, 19, "Canvas", Color.White);
            ConsoleHelper.DrawBoxedWord(14, 19, $"{activeCanvasCard.Value}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));
            ConsoleHelper.DrawBoxedWord(19, 19, $"{ColorRules.GetRuleByColor(activeCanvasCard.Color).RuleDescription}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));
        }

        private static void EraseCanvas()
        {
            ConsoleHelper.EraseSection(40, 3, 14, 19);
        }

        private static void DrawPlayerBoards(Red7Game red7Game)
        {
            // Loop through players and draw individual player boards
            for (int i = 0; i < red7Game.Players.Count; i++)
            {
                var player = red7Game.Players.ElementAt(i);

                ConsoleHelper.DrawBoxedWord(4 + (i * PlayerBoardWidth), 2, player.Name, player.Active ? Color.Yellow : Color.White);

                // Draw Palette cards
                ConsoleHelper.WriteAt(4 + (i * PlayerBoardWidth), 6, "Palette:", Color.White);
                ConsoleHelper.EraseSection(PlayerBoardWidth - 4, 3, 4 + (i * PlayerBoardWidth), 7);

                for (int k = 0; k < player.Palette.Cards.Count; k++)
                {
                    var card = player.Palette.Cards.ElementAt(k);
                    var cardColor = card.Color.GetConsoleColor();
                    var cardValue = $"│{card.Value}│";

                    // Palette boxed cards
                    for (int j = 0; j <= 2; j++)
                    {
                        if (j == 0) // Box tops
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 7, "┌─┐", cardColor);
                        else if (j == 1) // Box contents
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 8, cardValue, cardColor);
                        else if (j == 2) // Box bottoms
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 9, "└─┘", cardColor);
                    }
                }

                // Draw hand cards
                ConsoleHelper.WriteAt(4 + (i * PlayerBoardWidth), 11, "Hand:", Color.White);
                ConsoleHelper.EraseSection(PlayerBoardWidth - 4, 3, 4 + (i * PlayerBoardWidth), 12);

                for (int k = 0; k < player.Hand.Cards.Count; k++)
                {
                    var card = player.Hand.Cards.ElementAt(k);
                    var cardColor = OtherPlayerBoardsMasked && !player.Active ? Color.White : card.Color.GetConsoleColor();
                    var cardValue = OtherPlayerBoardsMasked && !player.Active ? "│X│" : $"│{card.Value}│";

                    // Hand boxed cards
                    for (int j = 0; j <= 2; j++)
                    {
                        if (j == 0) // Box tops
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 12, "┌─┐", cardColor);
                        else if (j == 1) // Box contents
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 13, cardValue, cardColor);
                        else if (j == 2) // Box bottoms
                            ConsoleHelper.WriteAt(4 + (k * CardWidth) + (i * PlayerBoardWidth), 14, "└─┘", cardColor);
                    }
                }

            } // End Player Board Loop
        }

        private static void WriteFigletTitle()
        {
            var root = FileHelper.GetApplicationRoot();
            string newPath = Path.GetFullPath(Path.Combine(root, @"..\"));

            FigletFont font = FigletFont.Load($"{newPath}/Red7.Console/FigletFonts/standard.flf");
            Figlet figlet = new Figlet(font);

            Console.Write(figlet.ToAscii("Red Seven"), Color.Red);
        }

        private static void SetBorderValues(int playerCount)
        {
            var width = (playerCount * PlayerBoardWidth) + 3;
            WidthValue = width < MinWidthValue ? MinWidthValue : width;
        }

        private static void WriteConsoleSectionBorder(Color color)
        {
            ConsoleHelper.DrawBorder(color, 4, 34, 59, 5);
        }

        private static void EraseActionHistorySection()
        {
            ConsoleHelper.EraseSection(78, 3, 5, 36);
        }

        private static void WriteActionHistorySectionBorder(Color color)
        {
            ConsoleHelper.DrawBorder(color, 4, 35, 80, 5);
        }

        private static void EraseActionInputSection()
        {
            ConsoleHelper.EraseSection(78, 3, 5, 41);
        }

        private static void WriteActionInputSectionBorder(Color color)
        {
            ConsoleHelper.DrawBorder(color, 4, 40, 80, 5);
        }

        private static void AddPlayerPrompt(Red7Game red7Game)
        {
            SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.AddPlayers).First().Active = false;
            WriteSetupMenu(red7Game);

            WriteConsoleSectionBorder(Color.Yellow);
            ConsoleHelper.WriteWordWrapAt(58, 7, 36, "Add Player: ", Color.White);
            Console.SetCursorPosition(19, 36);
            Console.CursorVisible = true;

            var playerName = Console.ReadLine();
            Console.CursorVisible = false;

            red7Game.Players.Add(new Player(playerName));
            SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.AddPlayers).First().Active = true;

            EraseOutputSection();
            WriteConsoleSectionBorder(Color.White);
            WriteSetupMenu(red7Game);
            EraseSettings();
            WriteSettings(red7Game);
        }

        

        private static void SelectCardToPlayToPalette(Red7Game game, Player activePlayer)
        {
            // Create a temporary hand menu defaulting to the first card as active
            var hand = new HandMenu() { MenuOptions = activePlayer.Hand.Cards.Select(x => new HandMenuOption(x, false)).ToList() };
            hand.MenuOptions.First().Active = true;

            // Write Prompt to Action Section
            WriteActionInputSectionBorder(Color.Yellow);
            ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
            ConsoleHelper.DrawBoxedWord(11, 41, hand.MenuOptions.First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.First().Card.Color));
            ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to your Palette", Color.White);

            var activePlayerIndex = game.Players.FindIndex(x => x.Active == true);
            WriteCurrentCardIndicator(0, activePlayerIndex);

            // Card select loop
            while (!Console.KeyAvailable)
            {
                var activeCard = hand.MenuOptions.Where(x => x.Active == true).First();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    hand.MenuOptions.ElementAt(index).Active = false;
                    if (index == 0)
                        hand.MenuOptions.ElementAt(hand.MenuOptions.Count - 1).Active = true;
                    else
                        hand.MenuOptions.ElementAt(index - 1).Active = true;

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();
                    var currentIndex = hand.MenuOptions.FindIndex(y => y.Active);

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    hand.MenuOptions.ElementAt(index).Active = false;
                    if (hand.MenuOptions.Count - 1 > index)
                        hand.MenuOptions.ElementAt(index + 1).Active = true;
                    else
                        hand.MenuOptions.ElementAt(0).Active = true;

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();
                    var currentIndex = hand.MenuOptions.FindIndex(y => y.Active);

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = hand.MenuOptions.Where(x => x.Active).First();

                    if (GameLogic.IsWinning(game, selectedOption.Card)) {
                        activePlayer.AddCardToPalette(selectedOption.Card);
                        activePlayer.RemoveCardFromHand(selectedOption.Card);

                        activePlayer.Active = false;
                        game.GetNextPlayer(activePlayer).Active = true;

                        // Write out as previous action in ActionOutputSection
                        EraseActionHistorySection();
                        ConsoleHelper.WriteWordWrapAt(78, 6, 37, $"{activePlayer.Name} played a", Color.White);
                        ConsoleHelper.DrawBoxedWord(6 + activePlayer.Name.Length + 10, 36, selectedOption.Card.Value.ToString(), ColorConverter.GetConsoleColor(selectedOption.Card.Color));
                        ConsoleHelper.WriteWordWrapAt(78, 6 + activePlayer.Name.Length + 10 + 3, 37, " to their Palette", Color.White);

                        WriteActionInputSectionBorder(Color.White);
                        EraseActionInputSection();
                        EraseCurrentCardIndicator(activePlayerIndex);

                        DrawPlayerBoards(game);
                        break;
                    }
                    else
                    {
                        EraseActionInputSection();
                        ConsoleHelper.WriteWordWrapAt(78, 24, 42, "Illegal Move (Press any key to continue)", Color.Yellow);

                        Console.ReadKey(true);
                        EraseActionInputSection();

                        ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
                        ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                        ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to your Palette", Color.White);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    WriteActionInputSectionBorder(Color.White);
                    EraseActionInputSection();
                    EraseCurrentCardIndicator(activePlayerIndex);
                    break;
                }

                // Empty the buffer if necessary
                if (Console.KeyAvailable)
                    Console.ReadKey(false);
            }
        }

        private static void SelectCardToPlayToCanvas(Red7Game game, Player activePlayer)
        {
            // Create a temporary hand menu defaulting to the first card as active
            var hand = new HandMenu() { MenuOptions = activePlayer.Hand.Cards.Select(x => new HandMenuOption(x, false)).ToList() };
            hand.MenuOptions.First().Active = true;

            // Write Prompt to Action Section
            WriteActionInputSectionBorder(Color.Yellow);
            ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
            ConsoleHelper.DrawBoxedWord(11, 41, hand.MenuOptions.First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.First().Card.Color));
            ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to the Canvas", Color.White);

            var activePlayerIndex = game.Players.FindIndex(x => x.Active == true);
            WriteCurrentCardIndicator(0, activePlayerIndex);

            // Card select loop
            while (!Console.KeyAvailable)
            {
                var activeCard = hand.MenuOptions.Where(x => x.Active == true).First();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    hand.MenuOptions.ElementAt(index).Active = false;
                    if (index == 0)
                        hand.MenuOptions.ElementAt(hand.MenuOptions.Count - 1).Active = true;
                    else
                        hand.MenuOptions.ElementAt(index - 1).Active = true;

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();
                    var currentIndex = hand.MenuOptions.FindIndex(y => y.Active);

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    hand.MenuOptions.ElementAt(index).Active = false;
                    if (hand.MenuOptions.Count - 1 > index)
                        hand.MenuOptions.ElementAt(index + 1).Active = true;
                    else
                        hand.MenuOptions.ElementAt(0).Active = true;

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();
                    var currentIndex = hand.MenuOptions.FindIndex(y => y.Active);

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = hand.MenuOptions.Where(x => x.Active).First();

                    if (GameLogic.IsWinning(game, selectedOption.Card.Color))
                    {
                        game.Canvas.AddCardToCanvas(selectedOption.Card);
                        activePlayer.RemoveCardFromHand(selectedOption.Card);

                        activePlayer.Active = false;
                        game.GetNextPlayer(activePlayer).Active = true;

                        // Write out as previous action in ActionOutputSection
                        EraseActionHistorySection();
                        ConsoleHelper.WriteWordWrapAt(78, 6, 37, $"{activePlayer.Name} played a", Color.White);
                        ConsoleHelper.DrawBoxedWord(6 + activePlayer.Name.Length + 10, 36, selectedOption.Card.Value.ToString(), ColorConverter.GetConsoleColor(selectedOption.Card.Color));
                        ConsoleHelper.WriteWordWrapAt(78, 6 + activePlayer.Name.Length + 10 + 3, 37, " to the Canvas", Color.White);

                        WriteActionInputSectionBorder(Color.White);
                        EraseActionInputSection();
                        EraseCurrentCardIndicator(activePlayerIndex);
                        DrawPlayerBoards(game);

                        EraseCanvas();
                        DrawCanvas(game);
                        break;
                    }
                    else
                    {
                        EraseActionInputSection();
                        ConsoleHelper.WriteWordWrapAt(78, 24, 42, "Illegal Move (Press any key to continue)", Color.Yellow);

                        Console.ReadKey(true);
                        EraseActionInputSection();

                        ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
                        ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                        ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to the Canvas", Color.White);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    WriteActionInputSectionBorder(Color.White);
                    EraseActionInputSection();
                    EraseCurrentCardIndicator(activePlayerIndex);
                    break;
                }

                // Empty the buffer if necessary
                if (Console.KeyAvailable)
                    Console.ReadKey(false);
            }
        }

        private static void SelectCardToPlayToPaletteThenCanvas(Red7Game game, Player activePlayer)
        {
            // Create a temporary hand menu defaulting to the first card as active
            var hand = new HandMenu() { MenuOptions = activePlayer.Hand.Cards.Select(x => new HandMenuOption(x, false)).ToList() };
            hand.MenuOptions.First().Active = true;

            // Write Prompt to Action Section
            WriteActionInputSectionBorder(Color.Yellow);
            ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
            ConsoleHelper.DrawBoxedWord(11, 41, hand.MenuOptions.First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.First().Card.Color));
            ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to your Palette and", Color.White);
            ConsoleHelper.WriteWordWrapAt(78, 39, 42, "to the Canvas", Color.White);

            HandMenuOption selectedPaletteCard = null;
            var selectedPaletteCardIndex = -1;

            var activePlayerIndex = game.Players.FindIndex(x => x.Active == true);
            WriteCurrentCardIndicator(0, activePlayerIndex);

            // Card select loop
            while (!Console.KeyAvailable)
            {
                var activeCard = hand.MenuOptions.Where(x => x.Active == true).First();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    var currentIndex = index;

                    do
                    {
                        hand.MenuOptions.ElementAt(index).Active = false;
                        if (index == 0)
                            hand.MenuOptions.ElementAt(hand.MenuOptions.Count - 1).Active = true;
                        else
                            hand.MenuOptions.ElementAt(index - 1).Active = true;

                        currentIndex = hand.MenuOptions.FindIndex(y => y.Active);
                        index = currentIndex;
                    } while (currentIndex == selectedPaletteCardIndex);

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    if (selectedPaletteCard == null)
                        ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                    else
                        ConsoleHelper.DrawBoxedWord(35, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    var index = hand.MenuOptions.FindIndex(y => y.Active);
                    var currentIndex = index;

                    do
                    {
                        hand.MenuOptions.ElementAt(index).Active = false;
                        if (hand.MenuOptions.Count - 1 > index)
                            hand.MenuOptions.ElementAt(index + 1).Active = true;
                        else
                            hand.MenuOptions.ElementAt(0).Active = true;

                        currentIndex = hand.MenuOptions.FindIndex(y => y.Active);
                        index = currentIndex;
                    } while (currentIndex == selectedPaletteCardIndex);

                    activeCard = hand.MenuOptions.Where(x => x.Active == true).First();

                    WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                    if (selectedPaletteCard == null)
                        ConsoleHelper.DrawBoxedWord(11, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                    else
                        ConsoleHelper.DrawBoxedWord(35, 41, activeCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(activeCard.Card.Color));
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = hand.MenuOptions.Where(x => x.Active).First();

                    if (selectedPaletteCard == null)
                    {
                        selectedPaletteCard = selectedOption;
                        var index = hand.MenuOptions.FindIndex(y => y.Active);

                        selectedPaletteCardIndex = index;

                        hand.MenuOptions.Where(x => x.Active).First().Active = false;

                        if (index == 0)
                            hand.MenuOptions.ElementAt(1).Active = true;
                        else
                            hand.MenuOptions.ElementAt(0).Active = true;


                        var currentIndex = hand.MenuOptions.FindIndex(y => y.Active);
                        WriteCurrentCardIndicator(currentIndex, activePlayerIndex);

                        ConsoleHelper.DrawBoxedWord(35, 41, hand.MenuOptions.Where(x => x.Active).First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.Where(x => x.Active).First().Card.Color));

                    } else
                    {
                        if (GameLogic.IsWinning(game, selectedPaletteCard.Card, selectedOption.Card))
                        {
                            activePlayer.AddCardToPalette(selectedPaletteCard.Card);
                            activePlayer.RemoveCardFromHand(selectedPaletteCard.Card);
                            game.Canvas.AddCardToCanvas(selectedOption.Card);
                            activePlayer.RemoveCardFromHand(selectedOption.Card);

                            activePlayer.Active = false;
                            game.GetNextPlayer(activePlayer).Active = true;

                            // Write out as previous action in ActionOutputSection
                            EraseActionHistorySection();
                            ConsoleHelper.WriteWordWrapAt(78, 6, 37, $"{activePlayer.Name} played a", Color.White);
                            ConsoleHelper.DrawBoxedWord(6 + activePlayer.Name.Length + 10, 36, selectedPaletteCard.Card.Value.ToString(), ColorConverter.GetConsoleColor(selectedPaletteCard.Card.Color));
                            ConsoleHelper.WriteWordWrapAt(78, 6 + activePlayer.Name.Length + 10 + 3, 37, " to their Palette and a", Color.White);
                            ConsoleHelper.DrawBoxedWord(6 + activePlayer.Name.Length + 10 + 27, 36, selectedOption.Card.Value.ToString(), ColorConverter.GetConsoleColor(selectedOption.Card.Color));
                            ConsoleHelper.WriteWordWrapAt(78, 6 + activePlayer.Name.Length + 10 + 31, 37, "to the Canvas", Color.White);

                            WriteActionInputSectionBorder(Color.White);
                            EraseActionInputSection();
                            EraseCurrentCardIndicator(activePlayerIndex);

                            EraseCanvas();
                            DrawCanvas(game);

                            DrawPlayerBoards(game);
                            break;
                        }
                        else
                        {
                            EraseActionInputSection();
                            ConsoleHelper.WriteWordWrapAt(78, 24, 42, "Illegal Move (Press any key to continue)", Color.Yellow);

                            Console.ReadKey(true);
                            EraseActionInputSection();

                            selectedPaletteCard = null;
                            hand.MenuOptions.Where(x => x.Active == true).First().Active = false;
                            hand.MenuOptions.ElementAt(0).Active = true;

                            // Write Prompt to Action Section
                            WriteActionInputSectionBorder(Color.Yellow);
                            WriteCurrentCardIndicator(0, activePlayerIndex);
                            ConsoleHelper.WriteWordWrapAt(78, 6, 42, "Play", Color.White);
                            ConsoleHelper.DrawBoxedWord(11, 41, hand.MenuOptions.First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.First().Card.Color));
                            ConsoleHelper.WriteWordWrapAt(78, 15, 42, "to your Palette and", Color.White);
                            ConsoleHelper.WriteWordWrapAt(78, 39, 42, "to the Canvas", Color.White);
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    if (selectedPaletteCard == null)
                    {
                        WriteActionInputSectionBorder(Color.White);
                        EraseActionInputSection();
                        EraseCurrentCardIndicator(activePlayerIndex);
                        break;
                    } else
                    {
                        selectedPaletteCard = null;
                        selectedPaletteCardIndex = -1;

                        hand.MenuOptions.Where(x => x.Active == true).First().Active = false;
                        hand.MenuOptions.First().Active = true;

                        WriteCurrentCardIndicator(0, activePlayerIndex);

                        ConsoleHelper.DrawBoxedWord(11, 41, hand.MenuOptions.First().Card.Value.ToString(), ColorConverter.GetConsoleColor(hand.MenuOptions.First().Card.Color));

                        ConsoleHelper.EraseSection(3, 3, 35, 41); // Erase Canvas Card Selection
                    }
                }

                // Empty the buffer if necessary
                if (Console.KeyAvailable)
                    Console.ReadKey(false);
            }
        }

        private static void WriteCurrentCardIndicator(int cardIndex, int activePlayerIndex)
        {
            EraseCurrentCardIndicator(activePlayerIndex);
            ConsoleHelper.WriteAt(5 + (activePlayerIndex * PlayerBoardWidth) + (3 * cardIndex), 15, "^", Color.Yellow);
        }

        private static void EraseCurrentCardIndicator(int activePlayerIndex)
        {
            ConsoleHelper.EraseSection(32, 1, 5 + (activePlayerIndex * PlayerBoardWidth), 15);
        }

        private static void ToggleActionRule(Red7Game red7Game)
        {
            var actionRule = red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First();
            red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First().Enabled = !actionRule.Enabled;

            WriteSetupMenu(red7Game);
            WriteSettings(red7Game);
        }

        private static void ToggleScoringRule(Red7Game red7Game)
        {
            var scoringRule = red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First();
            red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First().Enabled = !scoringRule.Enabled;

            WriteSetupMenu(red7Game);
            WriteSettings(red7Game);
        }

        private static void ToggleDiscardDrawRule(Red7Game red7Game)
        {
            var discardDrawRule = red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.DiscardDraw).First();
            red7Game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.DiscardDraw).First().Enabled = !discardDrawRule.Enabled;

            WriteSetupMenu(red7Game);
            WriteSettings(red7Game);
        }

        private static void EraseSettings()
        {
            ConsoleHelper.EraseSection(44, 34, 72, 3);
        }

        private static void WriteSettings(Red7Game game)
        {
            var yOrigin = 3;
            ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, "Players:", Color.White);

            yOrigin += 2;

            foreach (var player in game.Players)
            {
                ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, player.Name, Color.White);
                yOrigin++;
            }

            yOrigin++;
            ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, "Advanced Rules:", Color.White);

            yOrigin += 2;

            foreach (var rule in game.Rules)
            {
                ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, $"{(rule.Enabled ? "(Enabled) " : "(Disabled)")} {rule.AdvancedRule.GetDescription()}", Color.White);
                yOrigin++;
            }
        }

        private static void EraseMenu()
        {
            ConsoleHelper.EraseSection(59, 30, 4, 8);
        }

        private static void WriteSetupMenu(Red7Game game)
        {
            WriteIntroduction(58, 5, 9, Color.White);
            WriteAddPlayer(58, 5, 12, SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.AddPlayers).First().Active ? Color.Yellow : Color.White);
            WriteDiscardDrawRule(58, 5, 14, SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.DiscardDrawRule).First().Active ? Color.Yellow : Color.White, game);
            WriteScoringRule(58, 5, 18, SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.ScoringRule).First().Active ? Color.Yellow : Color.White, game);
            WriteActionRule(58, 5, 21, SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.ActionRule).First().Active ? Color.Yellow : Color.White, game);
            WriteGameStart(58, 5, 24, SetupMenu.MenuOptions.Where(x => x.Option == SetupOption.GameStart).First().Active ? Color.Yellow : Color.White);
        }

        private static void WriteActionMenu()
        {
            WritePaletteAction(80, 4, BoardHeightValue + 2, ActionMenu.MenuOptions.Where(x => x.Option == ActionOption.PlayToPalette).First().Active ? Color.Yellow : Color.White);
            WriteCanvasAction(80, 4, BoardHeightValue + 4, ActionMenu.MenuOptions.Where(x => x.Option == ActionOption.PlayToCanvas).First().Active ? Color.Yellow : Color.White);
            WritePaletteCanvasAction(80, 4, BoardHeightValue + 6, ActionMenu.MenuOptions.Where(x => x.Option == ActionOption.PlayToPaletteThenCanvas).First().Active ? Color.Yellow : Color.White);
            WriteFoldAction(80, 4, BoardHeightValue + 8, ActionMenu.MenuOptions.Where(x => x.Option == ActionOption.Fold).First().Active ? Color.Yellow : Color.White);
        }

        private static void EraseOutputSection()
        {
            ConsoleHelper.EraseSection(55, 3, 6, 34);
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
                ConsoleHelper.WriteWordWrapAt(width, left, top, "2) Enable  Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", color);
            else
                ConsoleHelper.WriteWordWrapAt(width, left, top, "2) Disable Discard Draw Rule (Draw when playing to the Canvas and the number of cards in your palette is less than the value of the card played)", color);
        }

        private static void WriteScoringRule(int width, int left, int top, Color color, Red7Game game)
        {
            if (!game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Scoring).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(width, left, top, "3) Enable  Scoring Rule (Score all the cards that meet the current rule at the end of a round)", color);
            else
                ConsoleHelper.WriteWordWrapAt(width, left, top, "3) Disable Scoring Rule (Score all the cards that meet the current rule at the end of a round)", color);
        }

        private static void WriteActionRule(int width, int left, int top, Color color, Red7Game game)
        {
            if (!game.Rules.Where(x => x.AdvancedRule == Core.Enums.AdvancedRule.Action).First().Enabled)
                ConsoleHelper.WriteWordWrapAt(width, left, top, "4) Enable  Action Rule (Forced actions when playing odd numbered cards to your palette)", color);
            else
                ConsoleHelper.WriteWordWrapAt(width, left, top, "4) Enable  Action Rule (Forced actions when playing odd numbered cards to your palette)", color);
        }

        private static void WriteGameStart(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "5) Start Game", color);
        }

        private static void WritePaletteAction(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "1) Play a card to your Palette", color);
        }

        private static void WriteCanvasAction(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "2) Play a card to the Canvas", color);
        }

        private static void WritePaletteCanvasAction(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "3) Play a card to your Palette and then the Canvas", color);
        }

        private static void WriteFoldAction(int width, int left, int top, Color color)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "4) Resign from this round", color);
        }
    }
}
