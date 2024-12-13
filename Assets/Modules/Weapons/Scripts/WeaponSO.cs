using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapon", order = 0)]
    public class WeaponSO : ScriptableObject
    {
        public Sprite Icon;
        public BattleEntity.BattleEntityType AttackType;
        
        [Min(0)]
        public int BaseDamage;

        [Min(0)]
        public float DamageRate = 1f;
    }
}
