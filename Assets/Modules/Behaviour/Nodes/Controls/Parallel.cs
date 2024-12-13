namespace BehaviourModule.Nodes.Controls
{
    /// <summary>
    /// Node that evaluates all children until one fails.
    /// </summary>
    /// <remarks>
    /// If no child failed, this node exits with <see cref="NodeState.RUNNING"/>.
    /// </remarks>
    public class Parallel : Node
    {
        #region Constructor

        /// <inheritdoc cref="Parallel"/>
        public Parallel(params Node[] children) : base(children) { }

        #endregion

        #region Node

        /// <inheritdoc/>
        protected override NodeState OnEvaluate()
        {
            bool anyRunning = false;
            
            foreach (Node child in this)
            {
                NodeState childState = child.Evaluate();

                // Exit if failed
                if (childState == NodeState.FAILURE) 
                    return NodeState.FAILURE;
                
                anyRunning |= childState == NodeState.RUNNING;
            }

            return anyRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        }

        #endregion
        
        #region Operator

        public static Parallel operator +(Parallel root, Node child)
        {
            root.Attach(child);
            return root;
        }

        #endregion
    }
}