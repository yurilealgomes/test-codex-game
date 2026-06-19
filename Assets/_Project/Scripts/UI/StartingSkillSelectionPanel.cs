using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class StartingSkillSelectionPanel : MonoBehaviour
    {
        private readonly List<StartingSkillCardUI> cards = new List<StartingSkillCardUI>();
        private GameObject root;
        private GameDatabase database;
        private Action<SkillData> onSelected;
        private int selectedIndex;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.TryGet(out database);
            Canvas canvas = ServiceLocator.Get<Canvas>();
            root = UIFactory.CreatePanel(canvas.transform, "Starting Skill Selection", new Color(0.02f, 0.025f, 0.035f, 0.96f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject;
            UIFactory.CreateText(root.transform, "Title", "Choose Your Starting Skill", 36, Color.white, TextAnchor.MiddleCenter, new Vector2(0.22f, 0.78f), new Vector2(0.78f, 0.88f), Vector2.zero, Vector2.zero);
            UIFactory.CreateText(root.transform, "Hint", "Select a skill, then begin the run.", 18, new Color(0.82f, 0.9f, 1f), TextAnchor.MiddleCenter, new Vector2(0.25f, 0.72f), new Vector2(0.75f, 0.77f), Vector2.zero, Vector2.zero);

            for (int i = 0; i < 6; i++)
            {
                GameObject cardObject = new GameObject("Starting Skill Card " + (i + 1));
                StartingSkillCardUI card = cardObject.AddComponent<StartingSkillCardUI>();
                card.Build(root.transform, i);
                cards.Add(card);
            }

            Hide();
        }

        private void Update()
        {
            if (root == null || !root.activeSelf || database == null || database.Skills.Count == 0)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                SelectIndex(selectedIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                SelectIndex(selectedIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                SelectIndex(selectedIndex - 3);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                SelectIndex(selectedIndex + 3);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmSelection();
            }
        }

        public void Show(Action<SkillData> selectedCallback)
        {
            onSelected = selectedCallback;
            root.SetActive(true);
            for (int i = 0; i < cards.Count; i++)
            {
                bool hasSkill = database != null && i < database.Skills.Count;
                cards[i].gameObject.SetActive(hasSkill);
                if (hasSkill)
                {
                    cards[i].Setup(database.Skills[i], SelectSkill);
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

        private void SelectIndex(int index)
        {
            if (database == null || database.Skills.Count == 0)
            {
                return;
            }

            selectedIndex = (index + database.Skills.Count) % database.Skills.Count;
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].SetSelected(i == selectedIndex);
            }
        }

        private void ConfirmSelection()
        {
            if (database != null && selectedIndex >= 0 && selectedIndex < database.Skills.Count)
            {
                SelectSkill(database.Skills[selectedIndex]);
            }
        }

        private void SelectSkill(SkillData skill)
        {
            Hide();
            if (onSelected != null)
            {
                onSelected.Invoke(skill);
            }
        }
    }
}
