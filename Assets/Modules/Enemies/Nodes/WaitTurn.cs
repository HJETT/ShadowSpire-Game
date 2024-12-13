using BehaviourModule.Nodes;

namespace Enemies.Node
{
    internal class WaitTurn : BehaviourModule.Nodes.Node
    {
        private int waitTurns;
        private int movesPerTurn;
        private int turnsRemaining;

        public WaitTurn(EnemySO self)
        {
            turnsRemaining = waitTurns = self.MovementSpeed switch
            {
                EnemyMovementSpeed.VERY_SLOW => 2,
                EnemyMovementSpeed.SLOW => 1,
                _ => 0
            };
            movesPerTurn = self.MovementSpeed switch
            {
                EnemyMovementSpeed.FAST => 2,
                EnemyMovementSpeed.VERY_FAST => 3,
                _ => 1
            };
        }

        protected override NodeState OnEvaluate()
        {
            turnsRemaining--;

            if (turnsRemaining >= 0)
                return NodeState.FAILURE;
            
            turnsRemaining = waitTurns;
            return NodeState.SUCCESS;
        }
    }
}
