using UnityEngine;

namespace Holoville.HOTween.Editor.Core
{
    internal static class HOGUIStyle
    {
        public const int VSpace = 6;
        public const int TinyToggleWidth = 15;
        public const int NormalButtonWidth = 80;
        public const int TinyButtonWidth = 20;
        public static GUIStyle TitleStyle;
        public static GUIStyle BoxStyleRegular;
        public static GUIStyle LabelCentered;
        public static GUIStyle LabelSmallStyle;
        public static GUIStyle LabelSmallItalicStyle;
        public static GUIStyle LabelBoldStyle;
        public static GUIStyle LabelItalicStyle;
        public static GUIStyle LabelBoldItalicStyle;
        public static GUIStyle LabelWordWrapStyle;
        public static GUIStyle BtNegStyle;
        public static GUIStyle BtTinyStyle;
        public static GUIStyle BtTinyNegStyle;
        public static GUIStyle BtLabelStyle;
        public static GUIStyle BtLabelBoldStyle;
        public static GUIStyle BtLabelErrorStyle;
        private static readonly Color NegativeColor = Color.red;
        private static bool _initialized;

        public static void InitGUI()
        {
            if (_initialized) return;
            
            _initialized = true;
            
            StoreGUIStyles();
        }

        public static GUIStyle Label(int size) => Label(size, FontStyle.Normal, Color.clear);

        public static GUIStyle Label(FontStyle fontStyle) => Label(-1, fontStyle, Color.clear);

        public static GUIStyle Label(Color color) => Label(-1, FontStyle.Normal, color);

        public static GUIStyle Label(int size, FontStyle fontStyle) => Label(size, fontStyle, Color.clear);

        public static GUIStyle Label(int size, Color color) => Label(size, FontStyle.Normal, color);

        public static GUIStyle Label(FontStyle fontStyle, Color color) => Label(-1, fontStyle, color);

        public static GUIStyle Label(int size, FontStyle fontStyle, Color color)
        {
            var guiStyle = new GUIStyle(GUI.skin.label);
            if (size != -1)
                guiStyle.fontSize = size;
            guiStyle.fontStyle = fontStyle;
            if (color != Color.clear)
                guiStyle.normal.textColor = color;
            return guiStyle;
        }

        public static void SetStyleNormalTextColors(GUIStyle style, Color color) =>
            style.normal.textColor = style.hover.textColor = style.active.textColor = color;

        public static void SetStyleOnTextColors(GUIStyle style, Color color) => 
            style.onNormal.textColor = style.onHover.textColor = style.onActive.textColor = color;

        private static void StoreGUIStyles()
        {
            BoxStyleRegular = new GUIStyle(GUI.skin.box)
            {
                margin = new RectOffset(0, 0, 0, 0),
                overflow = new RectOffset(1, 2, 0, 1),
                padding = new RectOffset(6, 6, 6, 6)
            };
            TitleStyle = Label(12, FontStyle.Bold);
            LabelCentered = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.LowerCenter
            };
            LabelSmallStyle = Label(10);
            LabelSmallStyle.margin = new RectOffset(0, 0, -6, 0);
            LabelSmallItalicStyle = new GUIStyle(LabelSmallStyle)
            {
                fontStyle = FontStyle.Italic
            };
            LabelBoldStyle = Label(FontStyle.Bold);
            LabelItalicStyle = Label(FontStyle.Italic);
            LabelBoldItalicStyle = Label(FontStyle.BoldAndItalic);
            LabelWordWrapStyle = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true
            };
            BtNegStyle = new GUIStyle(GUI.skin.button);
            SetStyleNormalTextColors(BtNegStyle, NegativeColor);
            BtTinyStyle = new GUIStyle(GUI.skin.button)
            {
                padding =
                {
                    left = -2,
                    right = 0,
                    top = 2
                }
            };
            BtTinyNegStyle = new GUIStyle(BtTinyStyle);
            SetStyleNormalTextColors(BtTinyNegStyle, NegativeColor);
            BtLabelStyle = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.UpperLeft,
                normal = {background = null},
                padding = {left = 2},
                margin = new RectOffset(0, 0, -2, -2),
                fontSize = 10
            };
            BtLabelBoldStyle = new GUIStyle(BtLabelStyle)
            {
                fontStyle = FontStyle.Bold
            };
            BtLabelErrorStyle = new GUIStyle(BtLabelBoldStyle);
            SetStyleNormalTextColors(BtLabelErrorStyle, NegativeColor);
        }
    }
}