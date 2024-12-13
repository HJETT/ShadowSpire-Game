using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Player
{
    public class PlayerInformation : MonoBehaviour
    {
        #region Weapon

        [Header("Weapon")]
        [SerializeField]
        private WeaponOption weaponOption;

        public void SetWeapon(WeaponInstance weapon) => weaponOption.LoadOption(new WeaponOptionData() { WeaponInstance = weapon });

        #endregion

        #region Health

        [Header("Health")]
        [SerializeField]
        private TextMeshProUGUI healthText;

        [SerializeField]
        private TextMeshProUGUI healthPopupText;

        [SerializeField]
        private Animator healthAnimator;

        public void SetHealth(int health, int maxHealth) => healthText.text = string.Format(
            "<sprite name=icon_heart> {0} / {1}",
            health,
            maxHealth
        );

        public void HitHealth(int amount)
        {
            healthPopupText.text = string.Format("-{0}", amount);
            healthAnimator.SetTrigger("hit");
        }

        #endregion

        #region Potion

        [Header("Potion")]
        [SerializeField]
        private TextMeshProUGUI potionCountText;

        public void SetPotionCount(int count) => potionCountText.text = string.Format("x{0}", count);

        #endregion

        #region Group

        [Header("Group")]
        [SerializeField]
        private HorizontalLayoutGroup groupUI;

        public IEnumerator OpenGroup(float time)
        {
            const float SPACING = 450;
            const int PADDING_LEFT = -40;

            float spacing = groupUI.spacing;
            int paddingLeft = groupUI.padding.left;

            float duration = 0;

            while (duration < time)
            {
                groupUI.spacing = Mathf.Lerp(spacing, SPACING, duration / time);
                groupUI.padding.left = (int)Mathf.Lerp(paddingLeft, PADDING_LEFT, duration / time);

                duration += Time.deltaTime;
                yield return null;
            }

            groupUI.spacing = SPACING;
            groupUI.padding.left = PADDING_LEFT;
        }

        public IEnumerator CloseGroup(float time)
        {
            const float SPACING = 0;
            const int PADDING_LEFT = 0;

            float spacing = groupUI.spacing;
            int paddingLeft = groupUI.padding.left;

            float duration = 0;

            while (duration < time)
            {
                groupUI.spacing = Mathf.Lerp(spacing, SPACING, duration / time);
                groupUI.padding.left = (int)Mathf.Lerp(paddingLeft, PADDING_LEFT, duration / time);

                duration += Time.deltaTime;
                yield return null;
            }

            groupUI.spacing = SPACING;
            groupUI.padding.left = PADDING_LEFT;
        }

        #endregion
    }
}
