using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Util
{
    public static partial class Utils
    {
        public static string ToHexCode(this Color color)
        {
            var r = Mathf.RoundToInt(color.r * 255);
            var g = Mathf.RoundToInt(color.g * 255);
            var b = Mathf.RoundToInt(color.b * 255);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        public static Color EditAlphaChannel(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }


        public static string ToColorString(this string text, Color color)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"<color={color.ToHexCode()}>");
            stringBuilder.Append(text);
            stringBuilder.Append("</color>");
            return stringBuilder.ToString();
        }

        public static Color ToRGB(this string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;
        }

        #region Color Extension

        public static Color Brown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(0.6f, 0.4f, 0.2f);
        }

        public static Color Lightpink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(255 / 255f, 182 / 255f, 193 / 255f);
        }

        public static Color Pink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(1f, 192 / 255f, 203 / 255f);
        }

        public static Color Crimson
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(220 / 255f, 20f / 255f, 60 / 255f);
        }

        public static Color DeepPink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(1f, 20 / 255f, 147 / 255f);
        }

        public static Color LemonYellow
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(1f, 247 / 255f, 0f);
        }

        public static Color SkyBlue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(135 / 255f, 206 / 255f, 235 / 255f);
        }

        public static Color Lavender
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(230 / 255f, 230 / 255f, 250 / 255f);
        }

        public static Color Thistle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(216 / 255f, 191 / 255f, 216 / 255f);
        }

        public static Color OliveGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(128 / 255f, 128 / 255f, 0f);
        }

        public static Color Coral
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(255 / 255f, 127 / 255f, 80 / 255f);
        }

        public static Color MintGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0.6f, 1f, 0.6f);
        }

        public static Color PeachPuff
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(1f, 218 / 255f, 185 / 255f);
        }

        public static Color Indigo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(75 / 255f, 0f, 130 / 255f);
        }

        public static Color Goldenrod
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(218 / 255f, 165 / 255f, 32 / 255f);
        }

        public static Color Ochre
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(125 / 255f, 113 / 255f, 80 / 255f);
        }

        public static Color SlateGray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(112 / 255f, 128 / 255f, 144 / 255f);
        }

        public static Color ForestGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(34 / 255f, 139 / 255f, 34 / 255f);
        }

        public static Color HotPink
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(1f, 105 / 255f, 180 / 255f);
        }

        public static Color DarkTurquoise
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0f, 206 / 255f, 209 / 255f);
        }

        public static Color Purple
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(170 / 255f, 0, 255 / 255f);
        }
        
        public static Color MediumPurple
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(147 / 255f, 112 / 255f, 219 / 255f);
        }

        public static Color DeepSkyBlue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(0f, 191 / 255f, 255 / 255f);
        }

        public static Color AntiqueWhite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(250 / 255f, 235 / 255f, 215 / 255f);
        }

        public static Color PapayaWhip
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(255 / 255f, 239 / 255f, 184 / 255f);
        }

        public static Color Auqamarin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new Color(127 / 255f, 1f, 170 / 255f);
        }

        public static Color SpringGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(60 / 255f, 179 / 255f, 113 / 255f);
        }

        public static Color LightGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(144 / 255f, 238 / 255f, 144 / 255f);
        }

        public static Color PineGreen
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(40 / 255f, 81 / 255f, 81 / 255f);
        }

        public static Color Teal
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(60 / 255f, 123 / 255f, 124 / 255f);
        }

        public static Color Aquamarine
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(81 / 255f, 162 / 255f, 163 / 255f);
        }

        #endregion
    }
}