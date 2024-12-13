using System.Collections;
using BehaviourModule.Interfaces;
using BehaviourModule.Nodes;
using BehaviourModule.Nodes.Controls;
using Enemies;
using Enemies.Node;
using Entities.Interfaces;
using Managers;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace Entities
{
    public class EnemyEntity : GridEntity, ITurnable, IMovable, IEventable, IVisualizable
    {
        #region Behavior Tree

        protected IVisualizable tree;

        protected NodeState UpdateTree()
        {
            if (this._root == null)
                return NodeState.FAILURE;

            this._root.Reset();
            return this._root.Evaluate();
        }

        #endregion

        #region IVisualizble

        private Node _root;

        /// <inheritdoc/>
        public Node GetRoot() => this._root;

        public void RebuildRoot()
        {
            Selector root = new();

            Sequence movement = new();

            movement += new WaitTurn(_data);

            if(_data.Pathing == EnemyPathing.DIRECT)
                movement += new GoToTarget(this, GameManager.Instance.player).Alias("GoToTarget");

            if(_data.Pathing == EnemyPathing.RANDOM)
                movement += new GoRandomPosition(this).Alias("GoRandomPosition");

            root += movement;

            this._root = root.Alias("Root");
        }

        #endregion

        #region Data

        private EnemySO _data;

        public EnemySO EnemyData
        {
            get => _data;
            set => SetData(value);
        }

        private void SetData(EnemySO data)
        {
            spriteRenderer.sprite = data.OverworldSprite;
            spriteRenderer.flipX = data.IsFlipped;
            spriteRenderer.color = BattleEntity.BattleEntity.GetTypeColor(data.Type);

            _data = data;

            this.RebuildRoot();
            this._root = this.GetRoot();
        }

        #endregion

        #region ITurnable

        /// <inheritdoc/>
        IEnumerator ITurnable.Think()
        {
            var state = UpdateTree();

            if(state == NodeState.FAILURE)
            {
                yield return null;
                yield break;
            }

            var nextMovement = _root.GetData<Movement?>("NextMovement");
            yield return nextMovement;
        }

        #endregion

        #region IEventable

        /// <inheritdoc/>
        public void OnEntityLand(GridEntity entity)
        {
            if (entity is PlayerEntity player)
            {
                OnPlayerTouched(player);
                return;
            }
        }

        /// <inheritdoc/>
        public void OnEntityLanded(GridEntity entity)
        {
            if (entity is PlayerEntity player)
            {
                OnPlayerTouched(player);
                return;
            }
        }

        private void OnPlayerTouched(PlayerEntity player)
        {
            // Needs to check if you aleady started a battle.
            // If the player lands on an enemy, this method will be called twice,
            // because the player calls OnEntityLanded and this entity calls OnEntityLand.
            GameManager.Instance.StartBattle(this, player);
        }

        #endregion

        #region IMovable

        /// <inheritdoc/>
        void IMovable.OnMoveStart(Movement movement)
        {
            if (_data.IsFlipped)
            {
                if (movement == Movement.LEFT)
                    movement = Movement.RIGHT;
                else if (movement == Movement.RIGHT)
                    movement = Movement.LEFT;
            }

            FlipByMovement(movement);
        }

        #endregion

    }
}