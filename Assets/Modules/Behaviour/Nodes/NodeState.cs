namespace BehaviourModule.Nodes
{
    /// <summary>
    /// Current state of the node
    /// </summary>
    public enum NodeState
    {
        /// <summary>The node has succeeded its task</summary>
        SUCCESS = 0,

        /// <summary>The node has failed its task</summary>
        FAILURE = 1,

        /// <summary>The node is processing its task</summary>
        RUNNING = 2,
        
        /// <summary>The node did not start its task</summary>
        NONE = 3
    }
}