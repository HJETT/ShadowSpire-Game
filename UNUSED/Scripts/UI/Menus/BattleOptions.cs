using System.Text;
using TMPro;
using UnityEngine;

namespace Battle.UI
{
    public class BattleOptions : SelectMenu
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private TextMeshProUGUI optionsText;

        #endregion

        private int selectedIndex = -1;
        private string[] OPTIONS;

        public void SetOptions(string[] options)
        {
            OPTIONS = options;
            selectedIndex = 0;
            UpdateText(selectedIndex);
        }

        public string GetSelectedOption() => OPTIONS[selectedIndex];

        private void UpdateText(int index)
        {
            StringBuilder text = new();

            for (int i = 0; i < OPTIONS.Length; i++)
            {
                bool selected = i == index;

                text.Append(
                    string.Format(
                        "<color={0}>{1}</color> {2} <color={0}>{3}</color>",
                        selected ? "#FFFFFFFF" : "#FFFFFF00",
                        '>',
                        OPTIONS[i],
                        '<'
                    )
                );

                if (i != OPTIONS.Length - 1)
                    text.Append("\t\t\t");
            }

            optionsText.text = text.ToString();
        }

        #region Select Menu

        public override void Clear()
        {
            UpdateText(-1);
            IsSelected = false;
        }

        public override void Move(Vector2 dir)
        {
            if (dir.x < 0)
                selectedIndex--;
            else if (dir.x > 0)
                selectedIndex++;

            if (selectedIndex < 0)
                selectedIndex = OPTIONS.Length - 1;
            else if (selectedIndex >= OPTIONS.Length)
                selectedIndex = 0;

            UpdateText(selectedIndex);
        }

        #endregion
    }
}