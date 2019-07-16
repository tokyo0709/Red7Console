﻿using Red7.Core.Components;
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

        private static bool IsWinningPalette(Color canvasColor, Palette activePlayerPalette, List<Palette> opponentPalettes, ColorRule colorRule)
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

        private static bool IsWinningRedRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
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

        private static bool IsWinningOrangeRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }

        private static bool IsWinningYellowRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }

        private static bool IsWinningGreenRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }

        private static bool IsWinningBlueRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }

        private static bool IsWinningIndigoRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }

        private static bool IsWinningVioletRule(Palette activePlayerPalette, List<Palette> opponentPalettes)
        {
            throw new NotImplementedException();
        }
    }
}
