using System.Collections;
using Battle.Options;
using BattleEntity;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    /// <summary>
    /// Class that serves as an intermediate between the UI and the manager
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        #region UI Options

        [Header("UI Options")]
        [SerializeField]
        private BattleOptions battleOptions;

        [SerializeField]
        private EnemyOptions enemyOptions;

        private UIOptions GetOptions<T>() where T : UIOptionData
        {
            var type = typeof(T);

            if (type == typeof(EnemyOptionData))
                return enemyOptions;

            if (type == typeof(BattleOptionData))
                return battleOptions;

            return null;
        }

        public void Load<T>(params T[] options) where T : UIOptionData => GetOptions<T>()?.LoadOptions(options);
        public void Move<T>(Vector2 dir) where T : UIOptionData => GetOptions<T>()?.Move(dir);
        public void Enter<T>() where T : UIOptionData => GetOptions<T>()?.Enter();
        public void Escape<T>() where T : UIOptionData => GetOptions<T>()?.Escape();
        public void ShowSelection<T>() where T : UIOptionData => GetOptions<T>()?.ShowSelection();
        public void HideSelection<T>() where T : UIOptionData => GetOptions<T>()?.HideSelection();
        public U GetSelection<T, U>() where T : UIOptionData where U : UIOption<T> => GetOptions<T>()?.GetSelection<T, U>();

        #endregion

        #region Spoiler

        [Header("Spoiler")]
        [SerializeField]
        private GameObject spoiler;

        public IEnumerator DisableSpoiler()
        {
            spoiler.SetActive(false);
            yield return null;
        }

        public IEnumerator EnableSpoiler()
        {
            const float FADE_TIME = 7f;

            spoiler.SetActive(true);
            var blackout = spoiler.GetComponent<Image>();
            var blackoutColor = blackout.color;
            blackoutColor.a = 0f;
            blackout.color = blackoutColor;

            for (int i = 1; i <= FADE_TIME; i++)
            {
                blackoutColor.a = 1f / FADE_TIME * i;
                blackout.color = blackoutColor;
                yield return new WaitForSeconds(0.2f);
            }
        }
        #endregion

        #region Enemy Attack

        [Header("Enemy Attack")]
        [SerializeField]
        private CanvasGroup battleOptionsGroup;

        [SerializeField]
        private Transform enemyAttackUI;

        public IEnumerator StartEnemyTurn(PlayerInformation playerInformation)
        {
            battleOptionsGroup.alpha = 1;
            battleOptionsGroup.gameObject.SetActive(true);

            for (int i = 0; i <= 4; i++)
            {
                battleOptionsGroup.alpha = 1f - 1f / 4f * i;
                yield return new WaitForSeconds(0.1f);
            }

            battleOptionsGroup.gameObject.SetActive(false);

            yield return playerInformation.OpenGroup(0.2f);

            enemyAttackUI.gameObject.SetActive(true);
        }

        public IEnumerator EndEnemyTurn(PlayerInformation playerInformation)
        {
            enemyAttackUI.gameObject.SetActive(false);

            yield return playerInformation.CloseGroup(0.2f);

            //playerOptionUI.gameObject.SetActive(true);

            battleOptionsGroup.alpha = 0;
            battleOptionsGroup.gameObject.SetActive(true);

            for (int i = 0; i <= 4; i++)
            {
                battleOptionsGroup.alpha = 1f / 4f * i;
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion
    }
}