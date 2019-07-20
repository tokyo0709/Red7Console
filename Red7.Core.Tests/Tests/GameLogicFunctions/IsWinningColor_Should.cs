using Red7.Core.Enums;
using Red7.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Red7.Core.Tests.Tests.GameLogicFunctions
{
    public class IsWinningColor_Should
    {
        [Fact]
        public void Return_True_If_Stronger_Color()
        {
            // Arrange
            var firstColor = Color.Red;
            var secondColor = Color.Blue;

            // Act
            var isWinning = GameLogic.IsWinningColor(firstColor, secondColor);

            // Assert
            Assert.True(isWinning);
        }

        [Fact]
        public void Throw_Exception_If_Colors_Equal()
        {
            // Arrange
            var firstColor = Color.Blue;
            var secondColor = Color.Blue;

            // Act/Assert
            Assert.Throws<Exception>(() => GameLogic.IsWinningColor(firstColor, secondColor));
        }
    }
}
