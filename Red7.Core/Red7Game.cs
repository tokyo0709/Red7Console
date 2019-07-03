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
        public Canvas Canvas { get; set; } = new Canvas() { Cards = new List<Card>() { new Card() { Color = Enums.Color.Red, Value = 0, Action = Enums.Action.None } } };
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

        public void BeginGame()
        {
            GameInProgress = true;
            DealHands();
            SetStartingPalettes();
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
    }
}
