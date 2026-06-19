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
        private UpgradeData[] currentUpgrades;
        private int selectedIndex;

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
            currentUpgrades = upgrades;
            root.SetActive(true);
            for (int i = 0; i < cards.Count; i++)
            {
                bool hasUpgrade = upgrades != null && i < upgrades.Length;
                cards[i].gameObject.SetActive(hasUpgrade);
                if (hasUpgrade)
                {
                    int cardIndex = i;
                    cards[i].Setup(upgrades[i], onSelected, () => SelectIndex(cardIndex));
                }
            }

            SelectIndex(0);
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private void Update()
        {
            if (root == null || !root.activeSelf || currentUpgrades == null || currentUpgrades.Length == 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectIndex(selectedIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectIndex(selectedIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                ConfirmSelection();
            }
        }

        private void SelectIndex(int index)
        {
            if (currentUpgrades == null || currentUpgrades.Length == 0)
            {
                return;
            }

            selectedIndex = (index + currentUpgrades.Length) % currentUpgrades.Length;
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetSelected(i == selectedIndex && i < currentUpgrades.Length);
            }
        }

        private void ConfirmSelection()
        {
            if (onSelected != null && selectedIndex >= 0 && selectedIndex < currentUpgrades.Length)
            {
                onSelected.Invoke(currentUpgrades[selectedIndex]);
            }
        }
    }
}
