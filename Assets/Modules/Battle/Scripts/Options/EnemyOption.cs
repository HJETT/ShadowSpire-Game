using BattleEntity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Battle.Options
{
    public class EnemyOptionData : UIOptionData
    {
        public WeaponInstance Weapon;
        public BattleEnemyEntity Entity;
    }

    /// <summary>
    /// Script that handles to display a single option
    /// </summary>
    public class EnemyOption : UIOption<EnemyOptionData>
    {
        #region Fields

        [Header("Fields")]
        [SerializeField]
        private Image sprite;

        [SerializeField]
        private Image shadow;

        [SerializeField]
        private TextMeshProUGUI types;

        #endregion

        #region API

        /// <inheritdoc/>
        protected override void OnLoadOption(EnemyOptionData option)
        {
            SetEntity(option.Entity);
            SetEffectiveness(option.Weapon, option.Entity);
            SpawnAnimation();
        }

        /// <inheritdoc/>
        public override void Select()
        {
            SetTargetted(true);
        }

        /// <inheritdoc/>
        public override void Deselect()
        {
            SetTargetted(false);
        }

        #endregion

        #region Entity

        private void SetEntity(BattleEnemyEntity entity)
        {
            entity.Hit.AddListener(OnHit);
            entity.Death.AddListener(OnDeath);

            types.text = BattleEntity.BattleEntity.GetIcons(entity.Type);

            SetHealth(entity.Health);

            var enemy = entity.Enemy;
            sprite.sprite = enemy.FightSprite;

            shadow.sprite = enemy.FightShadowSprite;
            shadow.enabled = shadow.sprite != null;
        }

        private void OnHit(int damage)
        {
            SetDamage(damage);
            SetHealth(loadedOption.Entity.Health);
            HitAnimation();
        }

        private void OnDeath(int damage)
        {
            SetDamage(damage);
            SetHealth(0);
            DeathAnimation();

            (parent as EnemyOptions)?.FindNextValid(Vector2.right);
        }

        #endregion

        #region Health

        [Header("Health")]
        [SerializeField]
        private TextMeshProUGUI health;

        [SerializeField]
        private TextMeshProUGUI healthPopup;

        private void SetHealth(int health) => this.health.text = string.Format("<sprite name=icon_heart> {0}", health);
        private void SetDamage(int damage) => healthPopup.text = string.Format("-{0}", damage);

        #endregion

        #region Animations

        [Header("Animations")]
        [SerializeField]
        private Animator animator;

        private void SpawnAnimation() => animator.SetTrigger("spawnIn");
        private void SetTargetAnimation(bool isTargetted) => animator.SetBool("targetted", isTargetted);
        private void HitAnimation() => animator.SetTrigger("hit");
        private void DeathAnimation() => animator.SetTrigger("death");

        #endregion

        #region Target

        [Header("Target")]
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private TextMeshProUGUI targetEffectiveness;

        private void SetEffectiveness(WeaponInstance weapon, BattleEnemyEntity entity)
        {
            float percent = entity.CalculateEffectiveness(weapon.GetAttackType());

            targetEffectiveness.text = string.Format(
                "<sprite name={0}> <color={1}>{2}</color>%",
                weapon.GetIcon().name,
                GetEffectivenessColor(percent),
                percent
            );
        }

        private void SetTargetted(bool isTarget)
        {
            target.SetActive(isTarget);
            SetTargetAnimation(isTarget);
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
    }
}