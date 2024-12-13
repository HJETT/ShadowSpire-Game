using System.Collections;
using CameraUtils;
using Dungeon.Generation;
using Enemies;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UtilsModule;
using Weapons;

namespace Managers
{
    public class GameManager : Singleton<GameManager>
    {
        private void Start()
        {
            StartLevel();
        }

        #region Player

        public PlayerEntity player;
        public bool IsPlayerDead;

        public void Defeat()
        {
            endBattleBackground.gameObject.SetActive(true);
            StartCoroutine(battleManager.DeadPlayerTextFadeIn(diedText));
        }

        #endregion

        #region Dungeon

        [Header("Dungeon")]
        [SerializeField]
        private DungeonManager dungeonManager;

        [SerializeField]
        private int levelIndex = 0;

        [SerializeField]
        private Camera dungeonCamera;

        public int overSeed;
        public bool useSeed;

        public DungeonResult Level { get; private set; }
        public bool IsLevelOver { get; private set; }

        public void StartLevel()
        {
            Level = null;
            IsLevelOver = false;

            int seed = useSeed ? overSeed : Random.Range(int.MinValue, int.MaxValue);
            overSeed = seed;

            var settings = new DungeonSettings
            {
                Index = levelIndex,
                Seed = seed,
                Width = Mathf.Min(4 + levelIndex * 2, 18),
                Height = Mathf.Min(4 + levelIndex * 2, 12),
                MinimumRoomHeight = levelIndex <= 1 ? 2 : 3,
                MinimumRoomWidth = levelIndex <= 1 ? 2 : 3,
                SliceCount = Mathf.FloorToInt(levelIndex * 1.2f),
                AddHighLoop = levelIndex >= 5,
                AddLowLoop = levelIndex >= 5
            };

            Debug.Log("Seed: " + seed);

            Level = dungeonManager.StartLevel(settings, player);
            dungeonCamera.transform.position = new Vector3(
                Level.Width / 2f,
                -(Level.Height / 2f),
                dungeonCamera.transform.position.z
            );
            InputManager.Instance.SwitchToPlayer();
        }

        public void EndLevel()
        {
            IsLevelOver = true;
            levelIndex++;
            InputManager.Instance.SwitchToUI();
            StartCoroutine(dungeonManager.EndLevel(levelIndex, levelIndex + 1, new System.Func<IEnumerator>(EndLevelCallback)));
        }

        private IEnumerator EndLevelCallback()
        {
            yield return EndLevelWeaponOffer();
            StartLevel();
            yield return null;
        }

        #endregion

        #region Turn

        [Header("Turn")]
        [SerializeField]
        private TurnManager turnManager;

        #endregion

        #region Battle

        [Header("Battle")]
        [SerializeField]
        private BattleManager battleManager;

        [SerializeField]
        private Image endBattleBackground;

        [SerializeField]
        private TextMeshProUGUI diedText;

        public bool IsInBattle { get; private set; }

        public void StartBattle(EnemyEntity enemyEntity, PlayerEntity playerEntity)
        {
            if (IsInBattle)
                return;

            IsInBattle = true;
            StartCoroutine(battleManager.StartBattle(enemyEntity, playerEntity));
            InputManager.Instance.SwitchToUI();
        }

        public void EndBattle(bool isVictory, EnemyEntity enemy)
        {
            IsInBattle = false;

            if (isVictory)
            {
                Destroy(enemy.gameObject);
                turnManager.StartTurn();
                InputManager.Instance.SwitchToPlayer();
            }
            else
                Defeat();

            SceneManager.UnloadSceneAsync("BattleScene");
        }

        #endregion

        #region Weapons

        [Header("Weapons")]
        public WeaponSO[] allWeapons;

        [SerializeField]
        private WeaponUI weaponUI;

        private IEnumerator EndLevelWeaponOffer()
        {
            if (Level.Index < DungeonGenerator.WEAPON_INDEX)
                yield return null;
            else
                yield return weaponUI.ShowWeapons();
        }

        #endregion

        #region Enemies

        [Header("Weapons")]
        public EnemySO[] allEnemies;

        #endregion

        #region Scenes

        public IEnumerator ReturnToTitle()
        {
            dungeonManager.ClearDungeon();
            yield return null;

            var title = SceneManager.LoadSceneAsync("TitleScreen", Camera2D.IsIn3D ? LoadSceneMode.Additive : LoadSceneMode.Single);

            while (!title.isDone)
                yield return null;

            var gameDeload = SceneManager.UnloadSceneAsync("Game");

            while (!gameDeload.isDone)
                yield return null;
        }


        #endregion

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}
