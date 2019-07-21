using Red7.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Red7.Core.Components
{
    public class Card
    {
        public int Value { get; set; }
        public Color Color { get; set; }
        public Enums.Action Action { get; set; }
    }

    public class CardComparer : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (x.Value > y.Value)
                return 1;
            if (x.Value < y.Value)
                return -1;
            if (x.Value == y.Value && x.Color > y.Color)
                return 1;
            if (x.Value == y.Value && x.Color == y.Color)
                return 0;
            if (x.Value == y.Value && x.Color < y.Color)
                return -1;
            
            throw new Exception("Comparison failed.");
        }
    }
}
