using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Red7.Core.Tests.Tests.GameLogicFunctions
{
    // Indigo Rule: Most Sequential Cards
    public class IsWinningIndigoRule_Should
    {
        private readonly Palette _activePlayerPalette;
        private readonly List<Palette> _opponentPalettes;

        public IsWinningIndigoRule_Should()
        {
            _activePlayerPalette = new Palette(1);
            _opponentPalettes = new List<Palette>();
        }

        [Fact]
        public void Return_False_If_Opponent_Has_More_Cards_Matching_Rule()
        {
            // Arrange
            _activePlayerPalette.Cards = new List<Card>()
            {
                new Card() { Color = Color.Red, Value = 4 },
                new Card() { Color = Color.Blue, Value = 3 },
                new Card() { Color = Color.Orange, Value = 6 },
                new Card() { Color = Color.Orange, Value = 7 },
                new Card() { Color = Color.Red, Value = 7 },
                new Card() { Color = Color.Red, Value = 1 },
            };

            _opponentPalettes.Add(new Palette(2)
            {
                Cards = new List<Card>()
                {
                    new Card() { Color = Color.Orange, Value = 2 },
                    new Card() { Color = Color.Green, Value = 3 },
                    new Card() { Color = Color.Yellow, Value = 4 },
                }
            });

            // Act
            var isWinning = GameLogic.IsWinningIndigoRule(_activePlayerPalette, _opponentPalettes);

            // Assert
            Assert.False(isWinning);
        }

        [Fact]
        public void Return_False_If_Matching_Cards_Count_Equal_And_Opponent_Has_Higher_Value_Card()
        {
            // Arrange
            _activePlayerPalette.Cards = new List<Card>()
            {
                new Card() { Color = Color.Red, Value = 2 },
                new Card() { Color = Color.Blue, Value = 3 },
                new Card() { Color = Color.Indigo, Value = 4 },
            };

            _opponentPalettes.Add(new Palette(2)
            {
                Cards = new List<Card>()
                {
                    new Card() { Color = Color.Orange, Value = 5 },
                    new Card() { Color = Color.Violet, Value = 6 },
                    new Card() { Color = Color.Yellow, Value = 7 },
                }
            });

            // Act
            var isWinning = GameLogic.IsWinningIndigoRule(_activePlayerPalette, _opponentPalettes);

            // Assert
            Assert.False(isWinning);
        }

        [Fact]
        public void Return_False_If_Matching_Cards_Count_Equal_And_High_Card_Equal_And_Opponent_Has_Higher_Color_Card()
        {
            // Arrange
            _activePlayerPalette.Cards = new List<Card>()
            {
                new Card() { Color = Color.Red, Value = 5 },
                new Card() { Color = Color.Blue, Value = 7 },
                new Card() { Color = Color.Indigo, Value = 6 },
            };

            _opponentPalettes.Add(new Palette(2)
            {
                Cards = new List<Card>()
                {
                    new Card() { Color = Color.Orange, Value = 5 },
                    new Card() { Color = Color.Violet, Value = 6 },
                    new Card() { Color = Color.Red, Value = 7 },
                }
            });

            // Act
            var isWinning = GameLogic.IsWinningIndigoRule(_activePlayerPalette, _opponentPalettes);

            // Assert
            Assert.False(isWinning);
        }

        [Fact]
        public void Return_True_If_Active_Player_Beats_All_Ties()
        {
            // Arrange
            _activePlayerPalette.Cards = new List<Card>()
            {
                new Card() { Color = Color.Red, Value = 2 },
                new Card() { Color = Color.Blue, Value = 3 },
                new Card() { Color = Color.Indigo, Value = 4 },
                new Card() { Color = Color.Indigo, Value = 5 },
            };

            _opponentPalettes.Add(new Palette(2)
            {
                Cards = new List<Card>()
                {
                    new Card() { Color = Color.Orange, Value = 5 },
                    new Card() { Color = Color.Violet, Value = 6 },
                    new Card() { Color = Color.Yellow, Value = 7 },
                }
            });

            // Act
            var isWinning = GameLogic.IsWinningIndigoRule(_activePlayerPalette, _opponentPalettes);

            // Assert
            Assert.True(isWinning);
        }
    }
}
