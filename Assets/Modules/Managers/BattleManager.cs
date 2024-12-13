using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Options;
using BattleEntity;
using Entities;
using TMPro;
using UnityEngine;
using Weapons;

namespace Managers
{
    public class BattleManager : MonoBehaviour
    {
        private EnemyEntity enemyEntity;
        private BattleEnemyEntity[] battleEnemyEntities;
        private BattlePlayerEntity battlePlayerEntity;
        private PlayerEntity playerEntity;

        #region Battle State

        private bool hasBattleEnded;

        public IEnumerator StartBattle(EnemyEntity enemyEntity, PlayerEntity playerEntity)
        {
            hasBattleEnded = false;

            // Initialize entities
            this.enemyEntity = enemyEntity;
            this.playerEntity = playerEntity;
            battlePlayerEntity = new BattlePlayerEntity(playerEntity);

            // Start battle transition
            yield return StartBattleTransition();

            // Find all elements
            yield return FindBattleUI();

            yield return FindEnemyProjectiles();

            // Load options
            LoadBattleOptions();
            LoadEnemyOptions(playerEntity.Weapon, GenerateEnemies(enemyEntity));

            // Disable spoiler
            yield return battleUI.DisableSpoiler();

            // Wait for spawn animation
            yield return new WaitForSeconds(2f);

            EnableBattleOption();
            AddInputs();
        }

        public void EndBattle(bool isVictory)
        {
            hasBattleEnded = true;
            StartCoroutine(EndBattleCoroutine(isVictory));
        }

        private IEnumerator EndBattleCoroutine(bool isVictory)
        {
            RemoveInputs();
            DisableBattleOption();
            DisableEnemyOption();

            yield return battleUI.EnableSpoiler();

            GameManager.Instance.EndBattle(isVictory, enemyEntity);
        }

        #endregion

        #region Battle UI

        private BattleUI battleUI;

        private IEnumerator FindBattleUI()
        {
            do
            {
                battleUI = FindObjectOfType<BattleUI>();
                yield return null;
            } while (battleUI == null);
        }

        public IEnumerator DeadPlayerTextFadeIn(TextMeshProUGUI text)
        {
            const float FADE_TIME = 4f;

            text.text = $"You died \n at Level {GameManager.Instance.Level.Index + 1}";
            text.gameObject.SetActive(true);
            var textColor = text.color;
            textColor.a = 0f;
            text.color = textColor;

            yield return new WaitForSeconds(0.5f);

            for (int i = 1; i <= FADE_TIME; i++)
            {
                textColor.a = 1f / FADE_TIME * i;
                text.color = textColor;
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(5);

            yield return GameManager.Instance.ReturnToTitle();
        }

        #endregion

        #region Battle Options

        private bool isSelectingBattleOption;

        private void LoadBattleOptions()
        {
            battleUI.Load(new BattleOptionData[] {
                new() {
                    Text = "Attack",
                    OnEnter = OnAttackPressed
                },
                new() {
                    Text = "Heal",
                    OnEnter = () => StartCoroutine(OnHealPressed())
                },
                //new()
                //{
                //    Text = "Nuke",
                //    OnEnter = () => EndBattle(true)
                //},
                //new()
                //{
                //    Text = "Death",
                //    OnEnter = () => EndBattle(false)
                //}
            });
        }

        private void OnAttackPressed()
        {
            DisableBattleOption();
            EnableEnemyOption();
        }

        private IEnumerator OnHealPressed()
        {
            if (!playerEntity.HasPotions())
                yield break;

            yield return HealPlayer(30);

            RemoveInputs();
            DisableBattleOption();

            yield return EnemyTurn();
        }

        private void EnableBattleOption()
        {
            isSelectingBattleOption = true;
            battleUI.ShowSelection<BattleOptionData>();
        }

        private void DisableBattleOption()
        {
            isSelectingBattleOption = false;
            battleUI.HideSelection<BattleOptionData>();
        }

        #endregion

        #region Player

        private IEnumerator HealPlayer(int amount)
        {
            playerEntity.ConsumePotion();
            playerEntity.Heal(amount);

            yield return new WaitForSeconds(0.5f);
        }

        public void DamagePlayer(int amount)
        {
            battlePlayerEntity.TakeDamage(amount);

            if (battlePlayerEntity.IsDead)
                EndBattle(false);
        }

        #endregion

        #region Enemy Options

        private bool isSelectingEnemyOption;

        private void LoadEnemyOptions(WeaponInstance weapon, params BattleEnemyEntity[] entities)
        {
            battleEnemyEntities = entities;
            var options = new EnemyOptionData[entities.Length];

            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new EnemyOptionData()
                {
                    Weapon = weapon,
                    Entity = entities[i],
                    OnEnter = () => StartCoroutine(OnEnemyConfirmed()),
                    OnEscape = OnEnemyEscape
                };
            }

            battleUI.Load(options);
        }

        private BattleEnemyEntity[] GenerateEnemies(EnemyEntity enemy)
        {
            var level = GameManager.Instance.Level.Index;
            var random = GameManager.Instance.Level.Random;
            var allEnemies = GameManager.Instance.allEnemies;

            var enemies = new List<BattleEnemyEntity>();

            if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 1)
                enemies.Add(new(allEnemies[random.Next(0, allEnemies.Length)]));

            enemies.Add(new(enemy.EnemyData));

            if (random.NextDouble() <= 0.9f && level - Dungeon.Generation.DungeonGenerator.ENEMY_ROOM_INDEX >= 2)
                enemies.Add(new(allEnemies[random.Next(0, allEnemies.Length)]));

            return enemies.ToArray();
        }

        private IEnumerator OnEnemyConfirmed()
        {
            RemoveInputs();
            DisableEnemyOption();

            var enemyOption = battleUI.GetSelection<EnemyOptionData, EnemyOption>();
            var enemy = enemyOption.GetOption();

            enemy.Entity.TakeAttack(enemy.Weapon);

            yield return new WaitForSeconds(0.7f);

            // All enemies dead, victory
            if (!VerifyEnemiesState())
            {
                EndBattle(true);
                yield break;
            }

            yield return EnemyTurn();
        }

        private void OnEnemyEscape()
        {
            isSelectingEnemyOption = false;
            battleUI.HideSelection<EnemyOptionData>();

            EnableBattleOption();
        }

        private void EnableEnemyOption()
        {
            isSelectingEnemyOption = true;
            battleUI.ShowSelection<EnemyOptionData>();
        }

        private void DisableEnemyOption()
        {
            isSelectingEnemyOption = false;
            battleUI.HideSelection<EnemyOptionData>();
        }

        private bool VerifyEnemiesState()
        {
            for (int i = 0; i < battleEnemyEntities.Length; i++)
            {
                // If at least one enemy alive
                if (!battleEnemyEntities[i].IsDead)
                    return true;
            }

            // If all enemies dead
            return false;
        }

        private IEnumerator EnemyTurn()
        {
            // Set up
            projectiles.SetupProjectiles(battleEnemyEntities, battlePlayerEntity, this);

            // Disable Player BattleUI
            yield return battleUI.StartEnemyTurn(playerEntity.playerInformation);

            // Execute enemy attacks
            yield return projectiles.SpawnProjectiles(5f);

            // Enable Player BattleUI
            yield return battleUI.EndEnemyTurn(playerEntity.playerInformation);

            // End attacks
            projectiles.CleanProjectiles();

            if (hasBattleEnded)
                yield break;

            EnableBattleOption();
            AddInputs();
        }

        #endregion

        #region Enemy Projectiles

        private EnemyProjectiles projectiles;

        private IEnumerator FindEnemyProjectiles()
        {
            do
            {
                projectiles = FindObjectOfType<EnemyProjectiles>(true);
                yield return null;
            } while (projectiles == null);
        }

        #endregion

        #region Battle Transition

        [Header("Battle Transition")]
        [SerializeField]
        private Material transitionMaterial;

        [SerializeField]
        private Texture[] transitionTextures;

        private IEnumerator StartBattleTransition() => BattleTransition.ExecuteTransition(transitionMaterial, transitionTextures, 0.9f);

#if UNITY_EDITOR
        // For keeping consistency in editor
        private void OnApplicationQuit() => BattleTransition.ResetMaterial(transitionMaterial);
#endif

        #endregion

        #region Inputs

        private void Move(Vector2 dir)
        {
            if (isSelectingBattleOption)
            {
                battleUI.Move<BattleOptionData>(dir);
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Move<EnemyOptionData>(dir);
                return;
            }
        }

        private void Enter()
        {
            if (isSelectingBattleOption)
            {
                battleUI.Enter<BattleOptionData>();
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Enter<EnemyOptionData>();
                return;
            }
        }

        private void Escape()
        {
            if (isSelectingBattleOption)
            {
                battleUI.Escape<BattleOptionData>();
                return;
            }

            if (isSelectingEnemyOption)
            {
                battleUI.Escape<EnemyOptionData>();
                return;
            }
        }

        /// <summary>
        /// Adds the inputs for this object
        /// </summary>
        private void AddInputs()
        {
            InputManager.Instance.OnMoveUI.AddListener(Move);
            InputManager.Instance.OnEnterUI.AddListener(Enter);
            InputManager.Instance.OnEscapeUI.AddListener(Escape);
        }

        /// <summary>
        /// Removes the inputs for this object
        /// </summary>
        private void RemoveInputs()
        {
            InputManager.Instance.OnMoveUI.RemoveListener(Move);
            InputManager.Instance.OnEnterUI.RemoveListener(Enter);
            InputManager.Instance.OnEscapeUI.RemoveListener(Escape);
        }

        #endregion
    }
}