using System;
using UnityEngine;

namespace _Scripts.Color_Mix
{
    public static class Colors
    {
        public static Color Red => new Color(0.8773585f, 0.07863116f, 0.07863116f);
        public static Color Yellow => new Color(1f, 0.8833333f, 0, 1f);
        public static Color Blue => new Color(0f, 0.2784314f, 0.6705883f, 1f);
        public static Color White => Color.white;
        public static Color Black => Color.black;
        public static Color Green => new Color(0.04107335f, 0.6698113f, 0.04107335f, 1f);
        public static Color Orange => new Color(1f, 0.446f, 0f, 1f);
        public static Color LightBlue => new Color(0f, 0.7686275f, 0.9176471f, 1f);
        public static Color Purple => new Color(0.4716981f, 0.1045746f, 0.3040981f, 1f);
        public static Color Pink => new Color(0.93f, 0.315f, 0.58f, 1f);
        public static Color DarkBlue => new Color(0f, 0.098f, 0.343f, 1f);
        public static Color LightYellow => new Color(1f, 0.9647059f, 0.5f, 1f);
        public static Color Burgundy => new Color(0.5f, 0f, 0f, 1f);
        public static Color Gray => Color.gray;

        public static Color EnumToColor(ColorsEnum color)
        {
            Color newColor;
            switch (color)
            {
                case ColorsEnum.Black: newColor = Black; break;
                case ColorsEnum.White: newColor = White; break;
                case ColorsEnum.Red: newColor = Red; break;
                case ColorsEnum.Yellow: newColor = Yellow; break;
                case ColorsEnum.Blue: newColor = Blue; break;
                case ColorsEnum.Green: newColor = Green; break;
                case ColorsEnum.Orange: newColor = Orange; break;
                case ColorsEnum.LightBlue: newColor = LightBlue; break;
                case ColorsEnum.Purple: newColor = Purple; break;
                case ColorsEnum.Pink: newColor = Pink; break;
                case ColorsEnum.DarkBlue: newColor = DarkBlue; break;
                case ColorsEnum.LightYellow: newColor = LightYellow; break;
                case ColorsEnum.Burgundy: newColor = Burgundy; break;
                default: newColor = Gray; break;
            }

            return newColor;
        }

        public static string GetColorName(ColorsEnum color)
        {
            string colorName;
            switch (color)
            {
                case ColorsEnum.Green: colorName = "Зеленый"; break;
                case ColorsEnum.Orange: colorName = "Оранжевый"; break;
                case ColorsEnum.LightBlue: colorName = "Голубой"; break;
                case ColorsEnum.Purple: colorName = "Фиолетовый"; break;
                case ColorsEnum.Pink: colorName = "Розовый"; break;
                case ColorsEnum.DarkBlue: colorName = "Темно Синий"; break;
                case ColorsEnum.Gray: colorName = "Серый"; break;
                case ColorsEnum.LightYellow: colorName = "Светло Желтый"; break;
                case ColorsEnum.Burgundy: colorName = "Бордовый"; break;
                default: colorName = "\"Цвет не опознан.\""; break;
            }

            return colorName;
        }
        
    }
    
    [Serializable]
    public enum ColorsEnum
    {
        Red,
        Yellow,
        Blue,
        Green,
        White,
        Black,
        Orange,
        LightBlue,
        Purple,
        Pink,
        DarkBlue,
        Gray,
        LightYellow,
        Burgundy
    }
}