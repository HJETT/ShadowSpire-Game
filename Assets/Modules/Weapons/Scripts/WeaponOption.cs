using Battle.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Weapons
{
    public class WeaponOptionData : UIOptionData
    {
        public WeaponInstance WeaponInstance;
        public string Subtext;
    }

    public class WeaponOption : UIOption<WeaponOptionData>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI type;

        [SerializeField]
        private TextMeshProUGUI damage;

        [SerializeField]
        private Image frame;

        [SerializeField]
        private TextMeshProUGUI subtext;

        #endregion

        #region UIOption

        /// <inheritdoc/>
        protected override void OnLoadOption(WeaponOptionData option)
        {
            var instance = option.WeaponInstance;

            icon.sprite = instance.GetIcon();
            type.text = BattleEntity.BattleEntity.GetIcons(instance.GetAttackType());
            damage.text = instance.GetDamage().ToString();
            subtext.text = option.Subtext;
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            frame.gameObject.SetActive(false);
            subtext.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public override void Select()
        {
            frame.gameObject.SetActive(true);
            subtext.gameObject.SetActive(true);
        }

        #endregion
    }
}
