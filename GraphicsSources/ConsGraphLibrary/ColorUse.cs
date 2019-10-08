using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ConsGraphLibrary
{
    public class ColorUse
    {
        public ColorUse (Color c)
        {
            color = c;
            flag = false;
        }

        public Color color { get; private set;}
        public bool flag { get; set; }
    }

    public class OuterColorUseList
    {
        public void FreeColor(Color color)
        {
            colorUseDictionary[color] = false;
        }
        
        public Color GetColor()
        {
            foreach (var color in colorUseDictionary.Keys)
            {
                if (!colorUseDictionary[color])
                {
                    colorUseDictionary[color] = true;
                    return color;
                }
                //if (color == colorUseDictionary.Last().Key)
                //{
                //    foreach (var c in colorUseDictionary.Keys)
                //    {
                //        colorUseDictionary[c] = false;
                //    }
                //    colorUseDictionary[Color.Blue] = true;
                //    return colorUseDictionary.First().Key;
                //}
            }
            //return colorUseDictionary.First().Key;
            throw new Exception("Превышено максимальное число параметров");
            return Color.Transparent;
        }

        public Dictionary<Color, bool> colorUseDictionary = new Dictionary<Color, bool>()
                                                                {
                                                                    //{Color.Transparent, false},
                                                                    {Color.Blue, false},
                                                                    {Color.DarkOrange, false},
                                                                    {Color.LimeGreen, false},
                                                                    {Color.BlueViolet, false},
                                                                    {Color.DarkRed, false},
                                                                    {Color.DimGray, false},
                                                                    {Color.DeepPink, false},
                                                                    {Color.Olive, false},
                                                                    {Color.Black, false},
                                                                    {Color.DarkBlue, false},
                                                                    {Color.DarkGreen, false},
                                                                    {Color.OrangeRed, false},
                                                                    {Color.RoyalBlue, false},
                                                                    {Color.Green, false},
                                                                    {Color.Indigo, false},
                                                                    {Color.CadetBlue, false},
                                                                    {Color.DarkOrchid, false},
                                                                    {Color.Chocolate, false},
                                                                    {Color.Navy, false},
                                                                    {Color.MediumVioletRed, false},
                                                                    {Color.DarkOliveGreen, false},
                                                                    {Color.Firebrick, false},
                                                                    {Color.DarkGray, false},
                                                                    {Color.ForestGreen, false},
                                                                    {Color.Purple, false},
                                                                    {Color.Coral, false},
                                                                    {Color.Brown, false},
                                                                    {Color.DarkGoldenrod, false},
                                                                    {Color.DarkCyan, false},
                                                                    {Color.DarkMagenta, false},
                                                                    {Color.DarkSlateBlue, false},
                                                                    {Color.Crimson, false},
                                                                    {Color.DarkSlateGray, false},
                                                                    {Color.DarkViolet, false},
                                                                    {Color.DodgerBlue, false},
                                                                    {Color.HotPink, false},
                                                                    {Color.Gray, false},
                                                                    {Color.MediumSeaGreen, false},
                                                                    {Color.Maroon, false},
                                                                    {Color.Fuchsia, false},
                                                                    {Color.MediumBlue, false},
                                                                    {Color.IndianRed, false},
                                                                    {Color.MediumTurquoise, false},
                                                                    {Color.Peru, false},
                                                                    {Color.SlateGray, false},
                                                                    {Color.Magenta, false},
                                                                    {Color.MediumPurple, false},
                                                                    {Color.BurlyWood, false},
                                                                    {Color.SeaGreen, false},
                                                                    {Color.SaddleBrown, false},
                                                                    {Color.MediumOrchid, false},
                                                                    {Color.MediumSlateBlue, false},
                                                                    {Color.RosyBrown, false},
                                                                    {Color.DarkKhaki, false},
                                                                    {Color.Sienna, false},
                                                                    {Color.Violet, false},
                                                                    {Color.MidnightBlue, false},
                                                                    {Color.Tomato, false},
                                                                    {Color.LightSeaGreen, false},
                                                                    {Color.Tan, false},
                                                                    {Color.LightSlateGray, false},
                                                                    {Color.CornflowerBlue, false},
                                                                    {Color.Orchid, false},
                                                                    {Color.SteelBlue, false},
                                                                    {Color.MediumAquamarine, false},
                                                                    {Color.OliveDrab, false},
                                                                    {Color.SlateBlue, false},
                                                                    {Color.YellowGreen, false},
                                                                    {Color.DeepSkyBlue, false},
                                                                    {Color.Aqua, false},
                                                                    {Color.SpringGreen, false},
                                                                    {Color.DarkSalmon, false},
                                                                    {Color.DarkSeaGreen, false},
                                                                    {Color.DarkTurquoise, false},
                                                                    {Color.Gold, false},
                                                                    {Color.LawnGreen, false},
                                                                    {Color.LightCoral, false},
                                                                    {Color.Lime, false},
                                                                    {Color.MediumSpringGreen, false},
                                                                    {Color.Red, false}
                                                                };
    }
}
