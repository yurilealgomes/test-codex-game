using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class LevelUpPanel : MonoBehaviour
    {
        private readonly List<UpgradeCardUI> cards = new List<UpgradeCardUI>();
        private GameObject root;
        private Action<UpgradeData> onSelected;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Level Up Panel", new Color(0.02f, 0.025f, 0.035f, 0.92f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Choose an Upgrade", 34, Color.white, TextAnchor.MiddleCenter, new Vector2(0.25f, 0.84f), new Vector2(0.75f, 0.93f), Vector2.zero, Vector2.zero);

            for (int i = 0; i < 3; i++)
            {
                GameObject cardObject = new GameObject("Upgrade Card " + (i + 1));
                UpgradeCardUI card = cardObject.AddComponent<UpgradeCardUI>();
                card.Build(root.transform, i);
                cards.Add(card);
            }

            Hide();
        }

        public void Show(UpgradeData[] upgrades, Action<UpgradeData> selectedCallback)
        {
            onSelected = selectedCallback;
            root.SetActive(true);
            for (int i = 0; i < cards.Count; i++)
            {
                bool hasUpgrade = upgrades != null && i < upgrades.Length;
                cards[i].gameObject.SetActive(hasUpgrade);
                if (hasUpgrade)
                {
                    cards[i].Setup(upgrades[i], onSelected);
                }
            }
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }
    }
}
