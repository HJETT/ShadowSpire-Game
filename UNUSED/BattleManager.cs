using System.Collections;
using Battle.UI;
using Enemies;
using UnityEngine;
using UnityEngine.SceneManagement;
using Weapons;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        public WeaponSO weapon;

        public void StartBattle()
        {
            StartCoroutine(StartBattleTransition());
        }

        public void EndBattle()
        {

        }

        public BattleUI battleUI;

        #region Battle Transition

        [Header("Battle Transition")]
        [SerializeField] Material transitionMat;
        [SerializeField] Texture[] transitionTextures;

        public EnemySO enemyTemp1;
        public EnemySO enemyTemp2;
        public EnemySO enemyTemp3;

        private IEnumerator StartBattleTransition()
        {
            yield return LoadRandomTransition();
            yield return TransitionFadeIn(0.9f);

            var loadScene = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

            while (!loadScene.isDone)
                yield return null;

            yield return new WaitForSeconds(1f);

            battleUI = FindObjectOfType<BattleUI>();
            battleUI.battleManager = this;
            battleUI.SetBattleOptions("ATTACK", "HEAL");

            yield return null; // Wait for everything to set up

            battleUI.StartBattle(enemyTemp1, enemyTemp2, enemyTemp3);

            yield return null;

            battleUI.SPOILER.SetActive(false);

            yield return null; // Wait for spoiler to get removed

            AddInputs();
        }

        private IEnumerator EndBattleTransition()
        {
            battleUI = null;
            yield return null;
        }

        #endregion

        #region Transition Material

        private void SetTransitionTexture(Texture texture) => transitionMat.SetTexture("_TransitionTex", texture);
        private void SetTransitionCutoff(float cutoff) => transitionMat.SetFloat("_Cutoff", cutoff);

        private IEnumerator LoadRandomTransition()
        {
            int rdmIndex = Random.Range(0, transitionTextures.Length);
            SetTransitionTexture(transitionTextures[rdmIndex]);

            yield return null; // Wait for load texture
        }

        private IEnumerator TransitionFadeIn(float duration)
        {
            float time = 0;

            while (time < duration)
            {
                SetTransitionCutoff(Mathf.Lerp(0, 1, time / duration));

                yield return null; // Wait 1 frame

                time += Time.deltaTime;
            }

            SetTransitionCutoff(1f);
            yield return null; // Wait 1 frame
        }

        private void ResetTransitionMaterial()
        {
            SetTransitionTexture(null);
            SetTransitionCutoff(0);
        }

        #endregion

        #region Weapon

        public WeaponSO GetWeapon() => weapon;

        public float GetEffectiveness(BattleEntity.BattleEntity entity) => entity.CalculateEffectiveness(GetWeapon().AttackType);

        #endregion

        #region Inputs

        private void AddInputs()
        {
            InputManager.Instance.OnMoveUI.AddListener(Move);
            InputManager.Instance.OnEnterUI.AddListener(Enter);
            InputManager.Instance.OnEscapeUI.AddListener(Escape);
        }

        private void RemoveInputs()
        {
            InputManager.Instance.OnMoveUI.RemoveListener(Move);
            InputManager.Instance.OnEnterUI.RemoveListener(Enter);
            InputManager.Instance.OnEscapeUI.RemoveListener(Escape);
        }

        private void Move(Vector2 dir) => battleUI?.Move(dir);
        private void Enter() => battleUI?.Enter();
        private void Escape() => battleUI?.Escape();

        #endregion

#if UNITY_EDITOR
        // For keeping consistency in editor
        private void OnApplicationQuit() => ResetTransitionMaterial();
#endif
    }

}
