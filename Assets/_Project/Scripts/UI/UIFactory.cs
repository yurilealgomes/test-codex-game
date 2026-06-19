using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public static class UIFactory
    {
        private static Font cachedFont;

        public static Font Font
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    if (cachedFont == null)
                    {
                        cachedFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    }
                }

                return cachedFont;
            }
        }

        public static RectTransform CreateRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        public static Image CreatePanel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = CreateRect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text CreateText(Transform parent, string name, string text, int fontSize, Color color, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = CreateRect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            Text label = rect.gameObject.AddComponent<Text>();
            label.font = Font;
            label.text = text;
            label.fontSize = fontSize;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            return label;
        }

        public static Button CreateButton(Transform parent, string name, string text, Color color, Action onClick, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            Image image = CreatePanel(parent, name, color, anchorMin, anchorMax, offsetMin, offsetMax);
            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            if (onClick != null)
            {
                button.onClick.AddListener(() => onClick.Invoke());
            }

            CreateText(image.transform, name + " Label", text, 20, Color.white, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return button;
        }

        public static Image CreateBar(Transform parent, string name, Color fillColor, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            Image background = CreatePanel(parent, name + " Background", new Color(0f, 0f, 0f, 0.55f), anchorMin, anchorMax, offsetMin, offsetMax);
            RectTransform fillRect = CreateRect(background.transform, name + " Fill", Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
            Image fill = fillRect.gameObject.AddComponent<Image>();
            fill.color = fillColor;
            fill.type = Image.Type.Simple;
            SetBarFill(fill, 1f);
            return fill;
        }

        public static void SetBarFill(Image fill, float value)
        {
            if (fill == null)
            {
                return;
            }

            float percent = Mathf.Clamp01(value);
            fill.fillAmount = percent;
            fill.enabled = percent > 0.001f;

            RectTransform rect = fill.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(percent, 1f);
            rect.offsetMin = new Vector2(2f, 2f);
            rect.offsetMax = new Vector2(-2f, -2f);
        }
    }
}
