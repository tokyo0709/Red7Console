using Red7.Core.Components;
using Red7.Core.Enums;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Red7.Core
{
    public class Red7Game
    {
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Rule> Rules { get; set; } = new List<Rule>();
        public DrawDeck Deck { get; set; } = new DrawDeck();
        public Canvas Canvas { get; set; } = new Canvas();
        public bool GameInProgress { get; set; } = false;
        private int StartingHandSize { get; } = 7;

        public Red7Game()
        {
            Rules.Add(new Rule { AdvancedRule = AdvancedRule.DiscardDraw, Enabled = false });
            Rules.Add(new Rule { AdvancedRule = AdvancedRule.Scoring, Enabled = false });
            Rules.Add(new Rule { AdvancedRule = AdvancedRule.Action, Enabled = false });
            

            Deck.Cards.AddRange(Seeder.GenerateCards());
            Deck.Shuffle();
        }

        public void AddPlayer(Player player)
        {
            if (GameInProgress) throw new Exception("Cannot add players in the middle of a game.");

            if (Players.Count < 4)
                Players.Add(player);
            else 
                throw new Exception("Player count cannot exceed 4 players.");
        }

        private void DealHands()
        {
            if (!GameInProgress) throw new Exception("Must begin playing in order to deal starting hands.");

            foreach (var player in Players)
            {
                for (int i = 0; i < StartingHandSize; i++)
                {
                    player.Hand.AddCardToHand(Deck.DrawCard());
                }
            }
        }

        private void SetStartingPalettes()
        {
            if (!GameInProgress) throw new Exception("Must begin playing in order to deal starting hands.");

            foreach (var player in Players)
            {
                player.Palette.AddCardToPalette(Deck.DrawCard());
            }
        }

        private void SetStartingPlayer()
        {
            foreach (var player in Players)
            {
                var canvasCard = Canvas.GetActiveCanvasCard();

                var isWinning = GameLogic.IsWinning(canvasCard.Color, player.Palette, Players.Where(x => x.Id != player.Id).ToList().Select(x => x.Palette).ToList());
                if (isWinning)
                {
                    var nextPlayer = GetNextPlayer(player);
                    nextPlayer.Active = true;
                }
            }
        }

        public void BeginGame()
        {
            GameInProgress = true;
            DealHands();
            SetStartingPalettes();
            SetStartingPlayer();
        }

        public void EnableDiscardDrawRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.DiscardDraw).First().Enabled = true;
        }

        public void DisableDiscardDrawRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.DiscardDraw).First().Enabled = false;
        }

        public void EnableScoringRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.Scoring).First().Enabled = true;
        }

        public void DisableScoringRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.Scoring).First().Enabled = false;
        }

        public void EnableActionRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.Action).First().Enabled = true;
        }

        public void DisableActionRule()
        {
            Rules.Where(x => x.AdvancedRule == AdvancedRule.Action).First().Enabled = false;
        }

        public Player GetNextPlayer(Player player)
        {
            int index = Players.FindIndex(x => x.Id == player.Id);

            if (index == -1) throw new Exception("Player not found.");

            if (Players.Count == index + 1)
                return Players.First();
            else
                return Players.ElementAt(index + 1);
        }
    }
}
