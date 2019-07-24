using Red7.Core.Components;

namespace Red7.ConsoleManager.Models
{
    public class SetupMenuOption
    {
        public SetupMenuOption(SetupOption option, bool active)
        {
            Option = option;
            Active = active;
        }

        public SetupOption Option { get; }
        public bool Active { get; set; }
    }

    public class ActionMenuOption
    {
        public ActionMenuOption(ActionOption option, bool active)
        {
            Option = option;
            Active = active;
            Legal = true;
        }

        public ActionOption Option { get; }
        public bool Active { get; set; }
        public bool Legal { get; set; }
    }

    public class HandMenuOption
    {
        public HandMenuOption(Card card, bool active)
        {
            Card = card;
            Active = active;
        }

        public Card Card { get; }
        public bool Active { get; set; }
    }
}
