using BattleEntity;
using Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Battle.UI
{
    public class EnemySlot : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image sprite;

        [SerializeField]
        private Image shadow;

        [SerializeField]
        private TextMeshProUGUI health;

        [SerializeField]
        private TextMeshProUGUI healthPopup;

        #endregion

        #region Data

        private BattleEnemyEntity entity;

        public void SetEnemy(EnemySO data)
        {
            if (data == null)
                ClearSlot();
            else
                SetSlot(data);
        }

        private void ClearSlot()
        {
            sprite.sprite = null;
            sprite.enabled = false;
            shadow.sprite = null;
            shadow.enabled = false;
            entity = null;
            UnTargetSlot();
        }

        private void SetSlot(EnemySO data)
        {
            sprite.sprite = data.FightSprite;
            sprite.enabled = true;
            shadow.sprite = data.FightShadowSprite;
            shadow.enabled = data.FightShadowSprite != null;
            entity = new BattleEnemyEntity(data);
            UnTargetSlot();
        }

        public BattleEnemyEntity GetEntity() => entity;

        #endregion

        #region Animations

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        public void SpawnAnimation() => animator.SetTrigger("spawnIn");
        public void HitAnimation(int damage)
        {
            animator.SetTrigger("hit");
            healthPopup.text = string.Format("-{0}", damage);
        }

        #endregion

        #region Target

        [Header("Target")]
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private TextMeshProUGUI targetEffectiveness;

        private void SetTargetted(bool isTarget)
        {
            target.SetActive(isTarget);
            animator.SetBool("targetted", isTarget);
        }

        public void TargetSlot() => SetTargetted(true);
        public void UnTargetSlot() => SetTargetted(false);

        public void SetEffectiveness(float percent, WeaponSO weapon)
        {
            targetEffectiveness.text = string.Format(
                "<sprite name={0}> <color={1}>{2}</color>%",
                weapon.Icon.name,
                GetEffectivenessColor(percent),
                percent
            );
        }

        private string GetEffectivenessColor(float percent)
        {
            if (percent >= 250)
                return "orange";

            if (percent >= 150)
                return "purple";

            if (percent > 100)
                return "green";

            if (percent == 100)
                return "white";

            if (percent >= 75)
                return "#A0A0A0";

            return "#505050";
        }

        #endregion

        #region Health

        public void SetHealth(int health) => this.health.text = string.Format("<sprite name=icon_heart> {0}", health);

        #endregion
    }
}

