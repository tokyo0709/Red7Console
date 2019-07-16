using Colorful;
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
        private static int HeightValue { get; set; } = 47;
        private static int BoardHeightValue { get; set; } = 24;
        private static int MinWidthValue { get; set; } = 120;
        private static int WidthValue { get; set; } = 200;
        private static int PlayerBoardWidth { get; set; } = 40;
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

        public static void InitializeConsoleSetup(Red7Game red7Game)
        {
            // Initialize Setup Console Sizing
            Console.SetWindowSize(120, 40);
            Console.SetBufferSize(120, 40);
            Console.CursorVisible = false;

            WriteFigletTitle();
            ConsoleHelper.DrawBorder(Color.Red, 1, 6, 65, 34, true); // Menu Border
            ConsoleHelper.DrawBorder(Color.White, 69, 0, 50, 40, true); // Setup Option Display Border

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
                else if (keyInfo.Key == ConsoleKey.Enter && SetupMenu.MenuOptions.Where(x => x.Active).First().Option == Option.GameStart)
                {
                    red7Game.BeginGame();
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = SetupMenu.MenuOptions.Where(x => x.Active).First();

                    switch (selectedOption.Option)
                    {
                        case Option.AddPlayers:
                            AddPlayerPrompt(red7Game);
                            break;
                        case Option.DiscardDrawRule:
                            ToggleDiscardDrawRule(red7Game);
                            break;
                        case Option.ScoringRule:
                            ToggleScoringRule(red7Game);
                            break;
                        case Option.ActionRule:
                            ToggleActionRule(red7Game);
                            break;
                        default:
                            break;
                    }
                }
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

            ConsoleHelper.DrawBorder(Color.FloralWhite, 0, 0, WidthValue, BoardHeightValue, true); // Player Board Section Border
            ConsoleHelper.DrawBorder(Color.White, 0, BoardHeightValue, WidthValue, HeightValue - BoardHeightValue, false); // Action Section Border
            WriteActionSectionBorder(Color.White); // Action Input/Output Section

            DrawRuleHelper(); // ROYGBIV Color Reference Helper
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
            ConsoleHelper.DrawBoxedWord(30, 16, "R", ColorConverter.GetConsoleColor(Core.Enums.Color.Red));
            ConsoleHelper.WriteAt(34, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(36, 16, "O", ColorConverter.GetConsoleColor(Core.Enums.Color.Orange));
            ConsoleHelper.WriteAt(40, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(42, 16, "Y", ColorConverter.GetConsoleColor(Core.Enums.Color.Yellow));
            ConsoleHelper.WriteAt(46, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(48, 16, "G", ColorConverter.GetConsoleColor(Core.Enums.Color.Green));
            ConsoleHelper.WriteAt(52, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(54, 16, "B", ColorConverter.GetConsoleColor(Core.Enums.Color.Blue));
            ConsoleHelper.WriteAt(58, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(60, 16, "I", ColorConverter.GetConsoleColor(Core.Enums.Color.Indigo));
            ConsoleHelper.WriteAt(64, 17, ">", Color.White);
            ConsoleHelper.DrawBoxedWord(66, 16, "V", ColorConverter.GetConsoleColor(Core.Enums.Color.Violet));

            var activeCanvasCard = red7Game.Canvas.GetActiveCanvasCard();
            ConsoleHelper.DrawBoxedWord(4, 19, "Canvas", Color.White);
            ConsoleHelper.DrawBoxedWord(14, 19, $"{activeCanvasCard.Value}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));
            ConsoleHelper.DrawBoxedWord(19, 19, $"{ColorRules.GetRuleByColor(activeCanvasCard.Color).RuleDescription}", ColorConverter.GetConsoleColor(activeCanvasCard.Color));

            Console.CursorVisible = false;

            Console.ReadLine();
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
            ConsoleHelper.DrawBorder(color, 5, 33, 57, 5);
        }

        private static void WriteActionSectionBorder(Color color)
        {
            ConsoleHelper.DrawBorder(color, 4, 39, 80, 5);
        }

        private static void AddPlayerPrompt(Red7Game red7Game)
        {
            SetupMenu.MenuOptions.Where(x => x.Option == Option.AddPlayers).First().Active = false;
            WriteSetupMenu(red7Game);

            WriteConsoleSectionBorder(Color.Yellow);
            ConsoleHelper.WriteWordWrapAt(58, 7, 35, "Add Player: ", Color.White);
            Console.SetCursorPosition(19, 35);
            Console.CursorVisible = true;

            var playerName = Console.ReadLine();
            Console.CursorVisible = false;

            red7Game.Players.Add(new Player(playerName));
            SetupMenu.MenuOptions.Where(x => x.Option == Option.AddPlayers).First().Active = true;

            EraseOutputSection();
            WriteConsoleSectionBorder(Color.White);
            WriteSetupMenu(red7Game);
            EraseSettings();
            WriteSettings(red7Game);
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

            yOrigin = yOrigin + 2;

            foreach (var player in game.Players)
            {
                ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, player.Name, Color.White);
                yOrigin++;
            }

            yOrigin++;
            ConsoleHelper.WriteWordWrapAt(45, 73, yOrigin, "Advanced Rules:", Color.White);

            yOrigin = yOrigin + 2;

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
            WriteAddPlayer(58, 5, 12, SetupMenu.MenuOptions.Where(x => x.Option == Option.AddPlayers).First().Active ? Color.Yellow : Color.White);
            WriteDiscardDrawRule(58, 5, 14, SetupMenu.MenuOptions.Where(x => x.Option == Option.DiscardDrawRule).First().Active ? Color.Yellow : Color.White, game);
            WriteScoringRule(58, 5, 18, SetupMenu.MenuOptions.Where(x => x.Option == Option.ScoringRule).First().Active ? Color.Yellow : Color.White, game);
            WriteActionRule(58, 5, 21, SetupMenu.MenuOptions.Where(x => x.Option == Option.ActionRule).First().Active ? Color.Yellow : Color.White, game);
            WriteGameStart(58, 5, 24, SetupMenu.MenuOptions.Where(x => x.Option == Option.GameStart).First().Active ? Color.Yellow : Color.White, game);
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

        private static void WriteGameStart(int width, int left, int top, Color color, Red7Game game)
        {
            ConsoleHelper.WriteWordWrapAt(width, left, top, "5) Start Game", color);
        }
    }
}
