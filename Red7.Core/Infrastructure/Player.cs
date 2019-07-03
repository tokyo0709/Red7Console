using Red7.Core.Components;

namespace Red7.Core.Infrastructure
{
    public class Player
    {
        public int Id { get; }
        public string Name { get; }
        public Hand Hand { get; }
        public Palette Palette { get; }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
            Hand = new Hand(id);
            Palette = new Palette(id);
        }

        public void AddCardToPalette(Card card)
        {
            Palette.Cards.Add(card);
        }
    }
}
