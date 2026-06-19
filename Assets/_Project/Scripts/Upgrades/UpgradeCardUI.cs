using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler
    {
        private Image background;
        private Text title;
        private Text description;
        private Text rarity;
        private Text affectedSkill;
        private Button button;
        private Outline outline;
        private Color baseColor;
        private Action hoverCallback;

        public void Build(Transform parent, int index)
        {
            RectTransform rect = gameObject.AddComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.08f + index * 0.31f, 0.18f);
            rect.anchorMax = new Vector2(0.30f + index * 0.31f, 0.82f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            background = gameObject.AddComponent<Image>();
            outline = gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.85f, 0.95f, 1f);
            outline.effectDistance = new Vector2(5f, 5f);
            outline.enabled = false;
            button = gameObject.AddComponent<Button>();
            button.targetGraphic = background;

            title = UIFactory.CreateText(transform, "Title", "", 22, Color.white, TextAnchor.UpperCenter, new Vector2(0.08f, 0.70f), new Vector2(0.92f, 0.95f), Vector2.zero, Vector2.zero);
            description = UIFactory.CreateText(transform, "Description", "", 15, Color.white, TextAnchor.UpperLeft, new Vector2(0.10f, 0.26f), new Vector2(0.90f, 0.67f), Vector2.zero, Vector2.zero);
            rarity = UIFactory.CreateText(transform, "Rarity", "", 16, Color.white, TextAnchor.MiddleCenter, new Vector2(0.12f, 0.09f), new Vector2(0.88f, 0.20f), Vector2.zero, Vector2.zero);
            affectedSkill = UIFactory.CreateText(transform, "Affected Skill", "", 14, new Color(0.85f, 0.9f, 1f), TextAnchor.MiddleCenter, new Vector2(0.08f, 0.21f), new Vector2(0.92f, 0.27f), Vector2.zero, Vector2.zero);
        }

        public void Setup(UpgradeData upgrade, Action<UpgradeData> onSelected, Action onHovered)
        {
            hoverCallback = onHovered;
            Color rarityColor = UpgradeRarityUtility.GetColor(upgrade.Rarity);
            baseColor = new Color(rarityColor.r * 0.45f, rarityColor.g * 0.45f, rarityColor.b * 0.45f, 0.96f);
            background.color = baseColor;
            title.text = upgrade.UpgradeName;
            description.text = UpgradeDescriptionBuilder.Build(upgrade);
            rarity.text = upgrade.Rarity.ToString();
            rarity.color = rarityColor;
            outline.effectColor = upgrade.Rarity == UpgradeRarity.Legendary ? new Color(1f, 0.78f, 0.18f) : new Color(0.85f, 0.95f, 1f);
            outline.effectDistance = upgrade.Rarity == UpgradeRarity.Legendary ? new Vector2(7f, 7f) : new Vector2(5f, 5f);
            affectedSkill.text = string.IsNullOrEmpty(upgrade.AffectedSkillLabel) ? "General Upgrade" : upgrade.AffectedSkillLabel;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onSelected.Invoke(upgrade));
        }

        public void SetSelected(bool selected)
        {
            if (outline != null)
            {
                outline.enabled = selected || rarity.text == UpgradeRarity.Legendary.ToString();
            }

            if (background != null)
            {
                background.color = selected ? Color.Lerp(baseColor, Color.white, 0.22f) : baseColor;
            }

            transform.localScale = selected ? Vector3.one * 1.04f : Vector3.one;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverCallback != null)
            {
                hoverCallback.Invoke();
            }
        }
    }
}
