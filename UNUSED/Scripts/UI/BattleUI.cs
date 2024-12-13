using Enemies;
using Managers;
using UnityEngine;

namespace Battle.UI
{
    public class BattleUI : MonoBehaviour
    {
        #region Fields

        [Header("Fields")]
        public GameObject SPOILER;

        #endregion

        #region Inputs

        public void Move(Vector2 dir)
        {
            var menus = new SelectMenu[] { battleOptions, enemyOptions };

            foreach (var item in menus)
            {
                if (item.IsSelected)
                {
                    item.Move(dir);
                    break;
                }
            }
        }

        public void Enter()
        {
            if (battleOptions.IsSelected)
            {
                var option = battleOptions.GetSelectedOption();

                if (option == "ATTACK")
                {
                    battleOptions.Clear();
                    enemyOptions.IsSelected = true;
                    Move(Vector2.zero);
                    return;
                }

                return;
            }

            if (enemyOptions.IsSelected)
            {
                var slot = enemyOptions.GetSelectedSlot();
                
                if (slot == null)
                    return;

                slot.UnTargetSlot();
                slot.HitAnimation(Mathf.RoundToInt(2 * battleManager.GetEffectiveness(slot.GetEntity()) / 100f));
                return;
            }
        }

        public void Escape()
        {
            if (enemyOptions.IsSelected)
            {
                enemyOptions.Clear();
                battleOptions.IsSelected = true;
                battleOptions.Move(Vector2.zero);
                return;
            }
        }

        #endregion

        #region Menus

        [Header("Menus")]
        [SerializeField]
        private BattleOptions battleOptions;

        [SerializeField]
        private EnemyOptions enemyOptions;

        [HideInInspector]
        public BattleManager battleManager;

        public void SetBattleOptions(params string[] options)
        {
            if (battleOptions == null)
                return;

            battleOptions.SetOptions(options);
            battleOptions.IsSelected = true;
        }

        public void StartBattle(EnemySO left, EnemySO middle, EnemySO right)
        {
            if (enemyOptions == null)
                return;

            enemyOptions.battleManager = battleManager;
            enemyOptions.SetSlot(left, 0);
            enemyOptions.GetSlot(0).SetHealth(20);
            enemyOptions.SetSlot(middle, 1);
            enemyOptions.GetSlot(1).SetHealth(10);
            enemyOptions.SetSlot(right, 2);
            enemyOptions.GetSlot(2).SetHealth(25);

            for (int i = 0; i < 3; i++)
            {
                var slot = enemyOptions.GetSlot(i);

                if (slot == null)
                    return;

                slot.SpawnAnimation();
            }
        }

        #endregion
    }
}
