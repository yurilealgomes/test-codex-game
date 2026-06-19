using System;
using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class StartingSkillCardUI : MonoBehaviour
    {
        private Image background;
        private Outline outline;
        private Text title;
        private Text description;
        private Text element;
        private SkillData skill;

        public void Build(Transform parent, int index)
        {
            RectTransform rect = gameObject.AddComponent<RectTransform>();
            rect.SetParent(parent, false);
            int column = index % 3;
            int row = index / 3;
            rect.anchorMin = new Vector2(0.10f + column * 0.28f, 0.48f - row * 0.30f);
            rect.anchorMax = new Vector2(0.34f + column * 0.28f, 0.72f - row * 0.30f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            background = gameObject.AddComponent<Image>();
            background.color = new Color(0.08f, 0.10f, 0.13f, 0.95f);
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.7f, 0.9f, 1f);
            outline.effectDistance = new Vector2(4f, 4f);
            outline.enabled = false;

            title = UIFactory.CreateText(transform, "Skill Name", "", 20, Color.white, TextAnchor.UpperCenter, new Vector2(0.06f, 0.68f), new Vector2(0.94f, 0.94f), Vector2.zero, Vector2.zero);
            description = UIFactory.CreateText(transform, "Skill Description", "", 14, new Color(0.84f, 0.9f, 1f), TextAnchor.UpperLeft, new Vector2(0.08f, 0.20f), new Vector2(0.92f, 0.66f), Vector2.zero, Vector2.zero);
            element = UIFactory.CreateText(transform, "Element", "", 13, Color.white, TextAnchor.MiddleCenter, new Vector2(0.10f, 0.06f), new Vector2(0.90f, 0.18f), Vector2.zero, Vector2.zero);
        }

        public void Setup(SkillData data, Action<SkillData> onSelected)
        {
            skill = data;
            title.text = data.SkillName;
            description.text = data.Description;
            element.text = data.ElementTags != null && data.ElementTags.Length > 0 ? data.ElementTags[0].ToString() : "Neutral";
            background.color = new Color(data.VisualColor.r * 0.35f, data.VisualColor.g * 0.35f, data.VisualColor.b * 0.35f, 0.96f);

            Button button = gameObject.GetComponent<Button>();
            if (button == null)
            {
                button = gameObject.AddComponent<Button>();
            }

            button.targetGraphic = background;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onSelected.Invoke(skill));
        }

        public void SetSelected(bool selected)
        {
            if (outline != null)
            {
                outline.enabled = selected;
            }

            transform.localScale = selected ? Vector3.one * 1.04f : Vector3.one;
        }
    }
}
