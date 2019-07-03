using Red7.Core.Enums;
using System;
using System.Text;

namespace Red7.Core.Components
{
    public class Card
    {
        public int Value { get; set; }
        public Color Color { get; set; }
        public Enums.Action Action { get; set; }
    }
}
