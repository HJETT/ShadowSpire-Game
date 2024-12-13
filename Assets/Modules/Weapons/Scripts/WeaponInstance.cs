using System;
using BattleEntity;
using Dungeon.Generation;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class WeaponInstance
    {
        #region Data

        private WeaponSO _data;
        private BattleEntityType _types;
        private int _level;
        private int _rdmDamageBoost;

        #endregion

        #region Constructors

        public WeaponInstance(WeaponSO weapon, int level)
        {
            _data = weapon;
            _level = level;
            _types = weapon.AttackType;
            _rdmDamageBoost = GameManager.Instance.Level.Random.Next(0, GameManager.Instance.Level.Index);
        }

        public static WeaponInstance CreateRandom(int level)
        {
            level -= DungeonGenerator.WEAPON_INDEX;

            var allWeapons = GameManager.Instance.allWeapons;
            var random = GameManager.Instance.Level.Random;

            var rdmWeapon = allWeapons[random.Next(0, allWeapons.Length)];
            var weapon = new WeaponInstance(rdmWeapon, level);

            if (level >= 3 && random.NextDouble() < 0.3f)
            {
                var types = Enum.GetValues(typeof(BattleEntityType));
                weapon._types |= (BattleEntityType) types.GetValue(random.Next(0, types.Length));
            }

            //weapon._types
            return weapon;
        }

        #endregion

        #region Getters

        public BattleEntityType GetAttackType() => _types;

        public int GetDamage() => Mathf.RoundToInt(_data.BaseDamage + _data.BaseDamage * 0.5f * (_level + _rdmDamageBoost * 0.4f) * _data.DamageRate);

        public Sprite GetIcon() => _data.Icon;

        #endregion
    }
}