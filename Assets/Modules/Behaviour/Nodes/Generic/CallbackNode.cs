namespace BehaviourModule.Nodes.Generic
{
    /// <summary>
    /// Node that executes the given action and returns a given state
    /// </summary>
    public class CallbackNode : Node
    {
        private readonly System.Func<Node, NodeState> CallBack;

        #region Constructor

        public CallbackNode(System.Action<Node> callback, NodeState state) : this(n =>
        {
            callback?.Invoke(n);
            return state;
        }) => this.SetMethodName(callback.Method);

        public CallbackNode(System.Func<NodeState> callback) : this(n => callback?.Invoke() ?? NodeState.FAILURE) 
            => this.SetMethodName(callback.Method);

        public CallbackNode(System.Func<Node, NodeState> callback)
        {
            this.CallBack = callback;
            this.SetMethodName(this.CallBack.Method);
        }

        #endregion

        #region Method Name

        private string MethodName;

        private void SetMethodName(System.Reflection.MethodInfo methodInfo)
        {
            string name = methodInfo.Name;
            this.MethodName = name.StartsWith('<') ? "Lambda" : name + "()";
        }

        #endregion

        #region Node

        /// <inheritdoc/>
        protected override NodeState OnEvaluate() => this.CallBack(this);

        /// <inheritdoc/>
        public override string GetText() => this.MethodName;

        #endregion
    }
}