using Red7.Core.Components;
using System.Linq;
using System.Threading;

namespace Red7.Core.Infrastructure
{
    public class Player
    {
        static int NextId;

        public int Id { get; }
        public string Name { get; }
        public Hand Hand { get; }
        public Palette Palette { get; }
        public bool Active { get; set; }

        public Player(string name)
        {
            Id = Interlocked.Increment(ref NextId);
            Name = name;
            Hand = new Hand(Id);
            Palette = new Palette(Id);
            Active = false;
        }

        public void AddCardToPalette(Card card)
        {
            Palette.Cards.Add(card);
        }

        public void RemoveCardFromHand(Card card)
        {
            Hand.Cards.RemoveAll(x => x.Color == card.Color && x.Value == card.Value);
        }
    }
}
