using PathFinding.Nodes;
using UnityEngine;

namespace PathFinding.Graphs
{
    public class RoomGraph : Graph<Node2D>
    {
        private int _nextNodeId;

        /// <summary>
        /// Adds a node to this graph
        /// </summary>
        /// <returns>ID of the node added</returns>
        public virtual int AddNode(Vector2 position)
        {
            var id = _nextNodeId;

            _nodes.Add(id, new Node2D(id, position));

            _nextNodeId++;

            return id;
        }

        /// <inheritdoc/>
        protected override float GetHeuristic(int start, int end)
        {
            var a = GetNode(start).Position;
            var b = GetNode(end).Position;

            return Vector2.Distance(a, b);
        }
    }
}