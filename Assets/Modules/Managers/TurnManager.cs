using System.Collections;
using System.Collections.Generic;
using Dungeon.Generation;
using Entities;
using Entities.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class TurnManager : MonoBehaviour, IDungeonReceive
    {
        private readonly List<GridEntity> turnEntities = new();
        private GridEntity[] foundEntities;

        private IEnumerator ProcessTurn()
        {
            bool _continue = true;
            while (_continue)
            {
                foreach (var entity in turnEntities)
                {
                    // If player is in battle or if the level is over, skip turn
                    if (GameManager.Instance.IsInBattle || GameManager.Instance.IsLevelOver || GameManager.Instance.IsPlayerDead)
                    {
                        _continue = false;
                        break;
                    }

                    // If entity doesnt exist, skip
                    if (entity == null)
                        continue;

                    yield return entity.ExecuteTurn();

                    // Check for event
                    foreach (var item in foundEntities)
                    {
                        // If entity doesnt exist, skip
                        if(item == null)
                            continue;

                        // If checking self, skip
                        if (item == entity)
                            continue;

                        // If not on the same position, skip
                        if (item.Position != entity.Position)
                            continue;

                        if (entity is IEventable landOn)
                            landOn.OnEntityLand(item);

                        if (item is IEventable landedOn)
                            landedOn.OnEntityLanded(entity);
                    }
                }
            }

            if (GameManager.Instance.IsPlayerDead)
                GameManager.Instance.Defeat();
        }


        public void StartTurn() => StartCoroutine(ProcessTurn());

        #region IDungeonReceive

        /// <inheritdoc/>
        public void OnLevelStart(DungeonResult level)
        {
            var player = level.Player;

            turnEntities.Clear();
            turnEntities.Add(player); // Make the player the first entity

            foundEntities = FindObjectsOfType<GridEntity>();

            foreach (var item in foundEntities)
            {
                // Don't add player twice
                if (item == player)
                    continue;

                if (item is not ITurnable)
                    continue;

                turnEntities.Add(item);
            }

            StartTurn();
        }

        /// <inheritdoc/>
        public void OnLevelEnd(DungeonResult level) { }

        #endregion
    }
}
