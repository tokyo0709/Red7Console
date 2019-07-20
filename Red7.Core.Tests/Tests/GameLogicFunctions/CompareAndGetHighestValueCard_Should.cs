using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Red7.Core.Tests.Tests.GameLogicFunctions
{
    public class CompareAndGetHighestValueCard_Should
    {
        [Fact]
        public void Return_Higher_Value_Card()
        {
            // Arrange
            var firstCard = new Card() { Color = Color.Blue, Value = 5 };
            var secondCard = new Card() { Color = Color.Blue, Value = 4 };

            // Act
            var winningCard = GameLogic.CompareAndGetHighestValueCard(firstCard, secondCard);

            // Assert
            Assert.Equal(firstCard, winningCard);
        }

        [Fact]
        public void Return_Higher_Color_Card_If_Value_Tied()
        {
            // Arrange
            var firstCard = new Card() { Color = Color.Blue, Value = 5 };
            var secondCard = new Card() { Color = Color.Red, Value = 5 };

            // Act
            var winningCard = GameLogic.CompareAndGetHighestValueCard(firstCard, secondCard);

            // Assert
            Assert.Equal(secondCard, winningCard);
        }

        [Fact]
        public void Throw_Exception_If_Value_And_Color_Are_Equal()
        {
            // Arrange
            var firstCard = new Card() { Color = Color.Blue, Value = 5 };
            var secondCard = new Card() { Color = Color.Blue, Value = 5 };

            // Act/Assert
            Assert.Throws<Exception>(() => GameLogic.CompareAndGetHighestValueCard(firstCard, secondCard));
        }
    }
}
