using BehaviourModule.Nodes.Abstract;
using UnityEngine;

namespace BehaviourModule.Nodes.Generic
{
    /// <summary>
    /// Node that succeeds when the target is at the given distance or more
    /// </summary>
    public sealed class DistanceGreater : DistanceNode
    {
        private readonly float _distance;

        #region Constructor

        public DistanceGreater(Transform self, string target, float distance) : base(self, target)
        {
            this._distance = distance;
        }

        #endregion

        #region DistanceNode

        /// <inheritdoc/>
        protected override NodeState GetState(float distance) => distance >= this._distance ? NodeState.SUCCESS : NodeState.FAILURE;

        /// <inheritdoc/>
        public override string GetText() => "Greater distance";

        #endregion
    }
}