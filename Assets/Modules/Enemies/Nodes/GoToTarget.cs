using BehaviourModule.Nodes;
using Entities;
using Managers;

namespace Enemies.Node
{
    internal class GoToTarget : BehaviourModule.Nodes.Node
    {
        private GridEntity self;
        private GridEntity target;
        private int[] path;
        private Movement[] movements;

        public GoToTarget(GridEntity self, GridEntity target)
        {
            this.self = self;
            this.target = target;    
        }

        protected override NodeState OnEvaluate()
        {
            path = PathFindingManager.FindPath(self, target);
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
    }
}
