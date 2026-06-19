using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ArcaneSurvival
{
    public sealed class MenuButtonNavigator : MonoBehaviour
    {
        private Button[] buttons = new Button[0];
        private Outline[] outlines = new Outline[0];
        private int selectedIndex;

        public void Configure(Button[] menuButtons)
        {
            buttons = menuButtons ?? new Button[0];
            outlines = new Outline[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == null)
                {
                    continue;
                }

                Outline outline = buttons[i].GetComponent<Outline>();
                if (outline == null)
                {
                    outline = buttons[i].gameObject.AddComponent<Outline>();
                }

                outline.effectColor = new Color(1f, 0.85f, 0.35f);
                outline.effectDistance = new Vector2(4f, 4f);
                outlines[i] = outline;
                AddHoverSelection(buttons[i], i);
            }

            SelectIndex(0);
        }

        private void Update()
        {
            if (!gameObject.activeInHierarchy || buttons.Length == 0)
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
            if (buttons.Length == 0)
            {
                return;
            }

            selectedIndex = (index + buttons.Length) % buttons.Length;
            for (int i = 0; i < outlines.Length; i++)
            {
                if (outlines[i] != null)
                {
                    outlines[i].enabled = i == selectedIndex;
                }
            }
        }

        private void ConfirmSelection()
        {
            if (selectedIndex >= 0 && selectedIndex < buttons.Length && buttons[selectedIndex] != null)
            {
                buttons[selectedIndex].onClick.Invoke();
            }
        }

        private void AddHoverSelection(Button button, int index)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            int capturedIndex = index;
            entry.callback.AddListener(_ => SelectIndex(capturedIndex));
            trigger.triggers.Add(entry);
        }
    }
}
