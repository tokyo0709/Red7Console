using Red7.Core.Components;
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
        public bool ActivePlayer { get; set; }

        public Player(string name)
        {
            Id = Interlocked.Increment(ref NextId);
            Name = name;
            Hand = new Hand(Id);
            Palette = new Palette(Id);
            ActivePlayer = false;
        }

        public void AddCardToPalette(Card card)
        {
            Palette.Cards.Add(card);
        }
    }
}
