namespace BehaviourModule.Nodes.Controls
{
    /// <summary>
    /// Node that inverts the state of its child. (NOT)
    /// </summary>
    /// <remarks>
    /// If the child's state is <see cref="NodeState.RUNNING"/> or the child is invalid, this node exits with <see cref="NodeState.RUNNING"/>.
    /// </remarks>
    public class Inverter : Node
    {
        #region Constructor

        /// <inheritdoc cref="Inverter"/>
        public Inverter(Node child) : base(child) { }

        #endregion

        #region Node

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            foreach (Node child in this)
            {
                NodeState childState = child.Evaluate();

                return childState switch
                {
                    NodeState.FAILURE => NodeState.SUCCESS,
                    NodeState.SUCCESS => NodeState.FAILURE,
                    _ => NodeState.RUNNING
                };
            }

            return NodeState.RUNNING;
        }

        /// <inheritdoc/>
        public override string GetText() => "NOT";

        #endregion
    }
}