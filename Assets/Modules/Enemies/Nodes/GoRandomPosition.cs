using BehaviourModule.Nodes;
using Entities;
using Managers;
using System.Collections.Generic;
using UnityEngine;
using UtilsModule;

namespace Enemies.Node
{
    internal class GoRandomPosition : BehaviourModule.Nodes.Node
    {
        private GridEntity self;
        private int[] path;
        private Movement[] movements;
        private Vector2Int rdmPosition;

        public GoRandomPosition(GridEntity self)
        {
            this.self = self;
            rdmPosition = self.Position;
        }

        protected override NodeState OnEvaluate()
        {
            if (self.Position == rdmPosition)
                FindRandomPosition();

            path = PathFindingManager.FindPath(self, rdmPosition);
            movements = PathFindingManager.GetDirections(path);

            // If no path found or on the same tile
            if (movements == null || movements.Length == 0)
            {
                SetData("NextMovement", null, -1);
                return NodeState.FAILURE;
            }

            SetData("NextMovement", movements[0], -1);

            return NodeState.SUCCESS;
        }

        private void FindRandomPosition()
        {
            var level = GameManager.Instance.Level;
            var rdmRoom = level.Rooms[level.Random.Next(0, level.Rooms.Length)];

            var positions = new List<Vector2Int>();

            for (int y = rdmRoom.Y; y < rdmRoom.Y + rdmRoom.Height; y++)
            {
                for (int x = rdmRoom.X; x < rdmRoom.X + rdmRoom.Width; x++)
                {
                    // If the tile is blocked, skip
                    if (level.IsBlocked(x, y))
                        continue;

                    positions.Add(new(x, -y));
                }
            }

            // If no valid position, skip
            if (positions.Count == 0)
                return;

            rdmPosition = positions[level.Random.Next(0, positions.Count)];
        }

    }
}
