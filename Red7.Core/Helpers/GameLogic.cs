using MoreLinq;
using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Red7.Core.Helpers
{

    public static class GameLogic
    {
        public static bool IsWinning(Red7Game game, Color canvasColorPlayed)
        {
            var colorRule = ColorRules.GetRuleByColor(canvasColorPlayed);
            var activePlayer = game.Players.Where(x => x.Active).First();

            var activePlayerPalette = activePlayer.Palette.CloneJson();
            var opponentPalettes = game.Players.Where(x => !x.Active).Select(y => y.Palette).ToList();

            return IsWinningPalette(canvasColorPlayed, activePlayerPalette, opponentPalettes, colorRule);
        }

        public static bool IsWinning(Red7Game game, Card cardPlayed)
        {
            var colorRule = ColorRules.GetRuleByColor(game.Canvas.Cards.Last().Color);
            var activePlayer = game.Players.Where(x => x.Active).First();

            var activePlayerPalette = activePlayer.Palette.CloneJson();
            activePlayerPalette.AddCardToPalette(cardPlayed);
            var opponentPalettes = game.Players.Where(x => !x.Active).Select(y => y.Palette).ToList();

            return IsWinningPalette(game.Canvas.Cards.Last().Color, activePlayerPalette, opponentPalettes, colorRule);
        }

        public static bool IsWinning(Red7Game game, Card cardPlayed, Card canvasColorPlayed)
        {
            var colorRule = ColorRules.GetRuleByColor(canvasColorPlayed.Color);
            var activePlayer = game.Players.Where(x => x.Active).First();

            var activePlayerPalette = activePlayer.Palette.CloneJson();
            activePlayerPalette.AddCardToPalette(cardPlayed);
            var opponentPalettes = game.Players.Where(x => !x.Active).Select(y => y.Palette).ToList();

            return IsWinningPalette(canvasColorPlayed.Color, activePlayerPalette, opponentPalettes, colorRule);
        }

        public static bool IsWinning(Color canvasColor, Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            var colorRule = ColorRules.GetRuleByColor(canvasColor);

            return IsWinningPalette(canvasColor, activePlayerPalette, opponentPalettes, colorRule);
        }

        public static bool IsWinningColor(Color activeColor, Color opponnentColor)
        {
            if (activeColor == opponnentColor)
                throw new Exception("Cannot compare the same color");

            return activeColor > opponnentColor;
        }

        public static Card CompareAndGetHighestValueCard(Card first, Card second)
        {
            if (first.Value > second.Value)
                return first;
            else if (first.Value == second.Value)
            {
                if ((int)first.Color > (int)second.Color)
                    return first;
                else if ((int)first.Color == (int)second.Color)
                    throw new System.Exception("Duplicate cards should not exist.");
                else
                    return second;
            }
            else
                return second;
        }

        public static bool IsWinningPalette(Color canvasColor, Palette activePlayerPalette, List<Palette> opponentPalettes, ColorRule colorRule)
        {
            switch (colorRule.Color)
            {
                case Color.Red:
                    return IsWinningRedRule(activePlayerPalette, opponentPalettes);
                case Color.Orange:
                    return IsWinningOrangeRule(activePlayerPalette, opponentPalettes);
                case Color.Yellow:
                    return IsWinningYellowRule(activePlayerPalette, opponentPalettes);
                case Color.Green:
                    return IsWinningGreenRule(activePlayerPalette, opponentPalettes);
                case Color.Blue:
                    return IsWinningBlueRule(activePlayerPalette, opponentPalettes);
                case Color.Indigo:
                    return IsWinningIndigoRule(activePlayerPalette, opponentPalettes);
                case Color.Violet:
                    return IsWinningVioletRule(activePlayerPalette, opponentPalettes);
                default:
                    throw new Exception($"No corresponding Color Rule for {canvasColor.ToString()}");
            }
        }

        public static bool IsWinningRedRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Highest Card
            var activePlayerHighestCard = activePlayerPalette.GetHighestCard();

            foreach (var palette in opponentPalettes)
            {
                var opponentHighestCard = palette.GetHighestCard();
                var winningCard = CompareAndGetHighestValueCard(activePlayerHighestCard, opponentHighestCard);
                if (winningCard.Color == opponentHighestCard.Color && winningCard.Value == opponentHighestCard.Value)
                    return false;
            }

            return true;
        }

        public static bool IsWinningOrangeRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most of One Number

            // Find the most frequent Value then if tied the highest value set
            var activePlayerHighOneNumberSet = activePlayerPalette.Cards
                .GroupBy(x => x.Value)
                .Select(group => new { Value = group.Key, Count = group.Count() })
                .OrderByDescending(y => y.Count)
                .ThenByDescending(z => z.Value)
                .First();

            foreach (var palette in opponentPalettes)
            {
                var currentPlayerHighOneNumberSet = palette.Cards
                    .GroupBy(x => x.Value)
                    .Select(group => new { Value = group.Key, Count = group.Count() })
                    .OrderByDescending(y => y.Count)
                    .ThenByDescending(z => z.Value)
                    .First();

                if (currentPlayerHighOneNumberSet.Count > activePlayerHighOneNumberSet.Count) return false;

                if (currentPlayerHighOneNumberSet.Count == activePlayerHighOneNumberSet.Count &&
                    currentPlayerHighOneNumberSet.Value > activePlayerHighOneNumberSet.Value) return false;
            }

            return true;
        }

        public static bool IsWinningYellowRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most of One Color

            // Find the most frequent Color then if tied the highest value set
            var activePlayerStrongColorGroup = GetStrongestColorGroup(activePlayerPalette);
            var activePlayerStrongestCard = activePlayerStrongColorGroup.MaxBy(y => y.Value).First();

            foreach (var palette in opponentPalettes)
            {
                var opponentStrongColorGroup = GetStrongestColorGroup(palette);
                var opponentStrongestCard = opponentStrongColorGroup.MaxBy(x => x.Value).First();

                // Total number matching the rule
                if (opponentStrongColorGroup.Count > activePlayerStrongColorGroup.Count) return false;

                if (opponentStrongColorGroup.Count == activePlayerStrongColorGroup.Count)
                {
                    // Highest value comparison
                    if (opponentStrongestCard.Value > activePlayerStrongestCard.Value) return false;

                    // Color comparison
                    if (opponentStrongestCard.Value == activePlayerStrongestCard.Value &&
                        opponentStrongestCard.Color > activePlayerStrongestCard.Color) return false;
                }
            }

            return true;
        }

        

        public static bool IsWinningGreenRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most Even Cards
            var activePlayerEvenCards = activePlayerPalette.Cards
                .Where(x => x.Value % 2 == 0)
                .OrderByDescending(y => y.Value)
                .ThenByDescending(z => z.Color)
                .ToList();

            foreach (var palette in opponentPalettes)
            {
                var currentPlayerEvenCards = palette.Cards
                    .Where(x => x.Value % 2 == 0)
                    .OrderByDescending(y => y.Value)
                    .ThenByDescending(z => z.Color)
                    .ToList();

                // Total number matching the rule
                if (currentPlayerEvenCards.Count > activePlayerEvenCards.Count) return false;

                // Highest value comparison
                if (currentPlayerEvenCards.Count == activePlayerEvenCards.Count)
                {
                    if (currentPlayerEvenCards.First().Value > activePlayerEvenCards.First().Value) return false;
                    if (currentPlayerEvenCards.First().Value == activePlayerEvenCards.First().Value &&
                        IsWinningColor(currentPlayerEvenCards.First().Color, activePlayerEvenCards.First().Color)) return false;
                }
            }

            return true;
        }

        public static bool IsWinningBlueRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most Different Colors

            // Get highest value unique color cards
            var activePlayerHighestValUniqueCards = activePlayerPalette.Cards
                .OrderByDescending(x => x.Value)
                .ThenByDescending(y => y.Color)
                .DistinctBy(z => z.Color)
                .ToList();

            foreach (var palette in opponentPalettes)
            {
                var currentPlayerHighestValUniqueCards = palette.Cards
                    .OrderByDescending(x => x.Value)
                    .ThenByDescending(y => y.Color)
                    .DistinctBy(z => z.Color)
                    .ToList();

                // Total number matching the rule
                if (currentPlayerHighestValUniqueCards.Count > activePlayerHighestValUniqueCards.Count) return false;

                // Highest value comparison
                if (currentPlayerHighestValUniqueCards.Count == activePlayerHighestValUniqueCards.Count)
                {
                    if (currentPlayerHighestValUniqueCards.First().Value > activePlayerHighestValUniqueCards.First().Value) return false;

                    // Color comparison
                    if (currentPlayerHighestValUniqueCards.First().Value == activePlayerHighestValUniqueCards.First().Value &&
                        IsWinningColor(currentPlayerHighestValUniqueCards.First().Color, activePlayerHighestValUniqueCards.First().Color)) return false;
                }
            }

            return true;
        }

        public static bool IsWinningIndigoRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most Cards in a Row
            var activePlayerSequentialList = GetSequentialCardsList(activePlayerPalette);
            var activePlayerOrderedSequentialList = activePlayerSequentialList
                .OrderByDescending(x => x.Count)
                .ToList();

            var activePlayerHighestCountList = activePlayerOrderedSequentialList.First();

            foreach (var palette in opponentPalettes)
            {
                var opponentSequentialList = GetSequentialCardsList(palette);
                var opponentOrderedSequentialList = opponentSequentialList
                    .OrderByDescending(x => x.Count)
                    .ToList();

                var opponentHighestCountList = opponentOrderedSequentialList.First();

                // Total number matching the rule
                if (opponentOrderedSequentialList.First().Count > activePlayerHighestCountList.Count) return false;

                // Highest value comparison
                if (opponentHighestCountList.Count == activePlayerHighestCountList.Count)
                {
                    if (opponentHighestCountList.First().Value > activePlayerHighestCountList.First().Value) return false;

                    // Color comparison
                    if (opponentHighestCountList.First().Value == activePlayerHighestCountList.First().Value &&
                        IsWinningColor(opponentHighestCountList.First().Color, activePlayerHighestCountList.First().Color)) return false;
                }
            }

            return true;
        }

        

        public static bool IsWinningVioletRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            // Most Cards Below 4
            var activePlayerHighestValueCardsBelowFour = activePlayerPalette.Cards
                .Where(x => x.Value < 4)
                .OrderByDescending(x => x.Value)
                .ThenByDescending(y => y.Color)
                .ToList();

            foreach (var palette in opponentPalettes)
            {
                var opponentHighestValueCardsBelowFour = palette.Cards
                    .Where(x => x.Value < 4)
                    .OrderByDescending(x => x.Value)
                    .ThenByDescending(y => y.Color)
                    .ToList();

                // Total number matching rule
                if (opponentHighestValueCardsBelowFour.Count > activePlayerHighestValueCardsBelowFour.Count) return false;

                // Highest value matching rule
                if (opponentHighestValueCardsBelowFour.Count == activePlayerHighestValueCardsBelowFour.Count)
                {
                    if (opponentHighestValueCardsBelowFour.First().Value > activePlayerHighestValueCardsBelowFour.First().Value) return false;

                    if (opponentHighestValueCardsBelowFour.First().Value == activePlayerHighestValueCardsBelowFour.First().Value &&
                        IsWinningColor(opponentHighestValueCardsBelowFour.First().Color, activePlayerHighestValueCardsBelowFour.First().Color)) return false;
                }
            }

            return true;
        }

        private static List<List<Card>> GetSequentialCardsList(Palette palette)
        {
            var activePlayerOrderedCards = palette.Cards
                            .OrderByDescending(x => x.Value)
                            .ThenByDescending(y => y.Color)
                            .ToList();

            // Get a list of sequential lists
            var listOfSequentialCards = new List<List<Card>>();
            var currentIndex = 0;

            foreach (var card in activePlayerOrderedCards)
            {
                // Check that we are still operating on the current sequential list
                if (listOfSequentialCards.Count == 0)
                {
                    // Need to generate a new sequential list
                    var newSeqList = new List<Card>();
                    newSeqList.Add(card);

                    listOfSequentialCards.Add(newSeqList);
                }
                else
                {
                    // Get current list at index
                    var currentList = listOfSequentialCards.ElementAt(currentIndex);

                    // Add card to list if it is one below the last cards value
                    if (currentList.Last().Value - 1 == card.Value)
                        currentList.Add(card);
                    else if (currentList.Last().Value - 1 > card.Value) // Increment index if more than 1 value apart
                    {
                        // Need to generate a new sequential list
                        var newSeqList = new List<Card>();
                        newSeqList.Add(card);

                        listOfSequentialCards.Add(newSeqList);
                        currentIndex++;
                    }
                }
            }

            return listOfSequentialCards;
        }

        private static List<Card> GetStrongestColorGroup(Palette palette)
        {
            var listOfGroupedColors = new List<List<Card>>();
            var orderedListOfGroupedColors = new List<List<Card>>();
            var cardsOrderedByColor = palette.Cards
                .OrderByDescending(x => x.Color)
                .ToList();

            var currentIndex = 0;

            foreach (var card in cardsOrderedByColor)
            {
                // Check that we are still operating on the current sequential list
                if (listOfGroupedColors.Count == 0)
                {
                    // Need to generate a new sequential list
                    var newColorList = new List<Card>();
                    newColorList.Add(card);

                    listOfGroupedColors.Add(newColorList);
                }
                else
                {
                    // Get current list at index
                    var currentList = listOfGroupedColors.ElementAt(currentIndex);

                    // Add card to list if it is one below the last cards value
                    if (card.Color == currentList.First().Color)
                        currentList.Add(card);
                    else // Increment index if different color
                    {
                        // Need to generate a new sequential list
                        var newColorList = new List<Card>();
                        newColorList.Add(card);

                        listOfGroupedColors.Add(newColorList);
                        currentIndex++;
                    }
                }
            }

            // Order grouped color lists
            foreach (var colorList in listOfGroupedColors)
            {
                orderedListOfGroupedColors.Add(colorList
                    .OrderByDescending(x => x.Value)
                    .ToList());
            }

            // Groups of equal count
            var highCountColorGroups = orderedListOfGroupedColors
                .MaxBy(x => x.Count)
                .ToList();

            var strongestColorGroup = new List<List<Card>>();

            // Check for high card or high color
            if (highCountColorGroups.Count > 1)
            {
                var highValueCards = new List<Card>();

                foreach (var group in highCountColorGroups)
                {
                    var highValueCard = group.MaxBy(x => x, new CardComparer()).First();
                    highValueCards.Add(highValueCard);
                }

                var highColorCard = highValueCards.MaxBy(x => x, new CardComparer()).First();
                foreach (var group in highCountColorGroups)
                {
                    if (group.Any(x => x.Color == highColorCard.Color && x.Value == highColorCard.Value))
                        strongestColorGroup.Add(group);
                }

                if (strongestColorGroup.Count > 1 || strongestColorGroup.Count <= 0)
                    throw new Exception("Must contain 1 element for the strongest color group.");
            } else
            {
                strongestColorGroup.Add(highCountColorGroups.First());
            }

            return strongestColorGroup.First();
        }
    }
}
