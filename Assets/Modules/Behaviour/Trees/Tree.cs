using BehaviourModule.Interfaces;
using BehaviourModule.Nodes;
using UnityEngine;

namespace BehaviourModule.Trees
{
    /// <summary>
    /// List of nodes to create a behaviour
    /// </summary>
    // Notion from here: https://www.youtube.com/watch?v=aR6wt5BlE-E
    public abstract class Tree : MonoBehaviour, IVisualizable
    {
        #region Tree

        private Node root;

        /// <summary>
        /// Called when this tree is being created
        /// </summary>
        /// <returns>Tree to use</returns>
        protected abstract Node SetUpTree();

        #endregion

        #region MonoBehaviour

        /// <inheritdoc cref="Start" />
        private void Start() => this.RebuildRoot();

        /// <inheritdoc cref="Update" />
        private void Update()
        {
            // Disable if root is invalid
            if (this.root == null)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.LogError("The root was invalid for '" + this.name + "'.");
                this.enabled = false;
                return;
            }

            this.root.Reset();
            this.root.Evaluate();
        }

        #endregion

        #region IVisualizable

        /// <inheritdoc/>
        public Node GetRoot() => this.root;

        /// <inheritdoc/>
        public void RebuildRoot() => this.root = this.SetUpTree();

        #endregion
    }
}