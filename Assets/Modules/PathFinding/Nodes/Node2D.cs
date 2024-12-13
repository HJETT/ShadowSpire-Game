using UnityEngine;

namespace PathFinding.Nodes
{
    public class Node2D : Node
    {
        public Vector2 Position { get; }

        public Node2D(int id, Vector2 position) : base(id)
        {
            this.Position = position;
        }
    }
}