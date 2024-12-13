using BehaviourModule.Nodes.Abstract;
using UnityEngine;

namespace BehaviourModule.Nodes.Generic
{
    /// <summary>
    /// Node that succeeds when the target is between two given distances
    /// </summary>
    public sealed class DistanceInBetween : DistanceNode
    {
        private readonly float minDistance;
        private readonly float maxDistance;

        #region Constructor

        public DistanceInBetween(Transform self, string target, float minDistance, float maxDistance) : base(self, target)
        {
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
        }

        #endregion

        #region DistanceNode

        /// <inheritdoc/>
        protected override NodeState GetState(float distance) => distance <= this.maxDistance && distance >= this.minDistance 
            ? NodeState.SUCCESS 
            : NodeState.FAILURE;

        /// <inheritdoc/>
        public override string GetText() => "In between distance";

        #endregion
    }
}