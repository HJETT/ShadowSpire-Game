namespace BehaviourModule.Nodes.Controls
{
    /// <summary>
    /// Node that succeed when any of its children succeed. (OR)
    /// </summary>
    /// <remarks>
    /// If a child succeed or is running, this node exits with child's state.
    /// </remarks>
    public class Selector : Node
    {
        #region Constructor

        /// <inheritdoc cref="Selector"/>
        public Selector(params Node[] children) : base(children) { }

        #endregion

        #region Node

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            foreach (Node child in this)
            {
                NodeState childState = child.Evaluate();

                // If child failed, skip
                if (childState == NodeState.FAILURE) 
                    continue;
                
                // Exit
                return childState;
            }

            return NodeState.FAILURE;
        }
        
        /// <inheritdoc/>
        public override string GetText() => "OR";

        #endregion
        
        #region Operator

        public static Selector operator +(Selector root, Node child)
        {
            root.Attach(child);
            return root;
        }

        #endregion
    }
}