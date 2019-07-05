namespace Red7.ConsoleManager.Models
{
    public class MenuOption
    {
        public MenuOption(Option option, bool active)
        {
            Option = option;
            Active = active;
        }

        public Option Option { get; }
        public bool Active { get; set; }
    }
}
