using BehaviourModule.Nodes.Abstract;
using UnityEngine;

namespace BehaviourModule.Nodes.Generic
{
    /// <summary>
    /// Node that succeeds when the target is at the given distance or less
    /// </summary>
    public sealed class DistanceSmaller : DistanceNode
    {
        private readonly float _distance;

        #region Constructor

        public DistanceSmaller(Transform self, string target, float distance) : base(self, target)
        {
            this._distance = distance;
        }

        #endregion

        #region DistanceNode

        /// <inheritdoc/>
        protected override NodeState GetState(float distance) => distance <= this._distance ? NodeState.SUCCESS : NodeState.FAILURE;

        /// <inheritdoc/>
        public override string GetText() => "Smaller distance";

        #endregion
    }
}