using Red7.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Red7.Core.Infrastructure
{
    public static class ColorRules
    {
        public static ColorRule Red
        {
            get
            {
                return new ColorRule(Color.Red, "Highest Card");
            }
        }

        public static ColorRule Orange
        {
            get
            {
                return new ColorRule(Color.Orange, "Most Of One Number");
            }
        }

        public static ColorRule Yellow
        {
            get
            {
                return new ColorRule(Color.Yellow, "Most Of One Color");
            }
        }

        public static ColorRule Green
        {
            get
            {
                return new ColorRule(Color.Green, "Most Even Cards");
            }
        }

        public static ColorRule Blue
        {
            get
            {
                return new ColorRule(Color.Blue, "Most Different Colors");
            }
        }

        public static ColorRule Indigo
        {
            get
            {
                return new ColorRule(Color.Indigo, "Most Cards In A Row");
            }
        }

        public static ColorRule Violet
        {
            get
            {
                return new ColorRule(Color.Violet, "Most Cards Below 4");
            }
        }

        public static ColorRule GetRuleByColor(Color color)
        {
            switch (color)
            {
                case Color.Red:
                    return Red;
                case Color.Orange:
                    return Orange;
                case Color.Yellow:
                    return Yellow;
                case Color.Green:
                    return Green;
                case Color.Blue:
                    return Blue;
                case Color.Indigo:
                    return Indigo;
                case Color.Violet:
                    return Violet;
                default:
                    throw new Exception($"No corresponding color matching {color.ToString()}");
            }
        }
    }

    public class ColorRule
    {
        public Color Color { get; }
        public string RuleDescription { get; }

        public ColorRule(Color color, string description)
        {
            Color = color;
            RuleDescription = description;
        }
    }
}
