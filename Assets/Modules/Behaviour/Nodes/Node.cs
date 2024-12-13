using BehaviourModule.Nodes.Generic;
using BehaviourModule.Nodes.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourModule.Nodes
{
    /// <summary>
    /// Class that represents a single element in the tree
    /// </summary>
    public abstract class Node : IEnumerable<Node>
    {
        #region Constructor

        public Node(params Node[] children)
        {
            this.Attach(children);
        }

        #endregion

        #region Data

        private readonly Dictionary<string, object> dataContext = new();

        /// <summary>
        /// Stores the given value at the given key
        /// </summary>
        public void SetData(string key, object value, int levels = 0)
        {
            Node node = this;

            if (levels < 0)
                levels = int.MaxValue;

            while (levels > 0)
            {
                // If root reached, skip
                if (node.parent == null)
                    break;

                node = node.parent;
                levels--;
            }

            // Set value inside current
            node.dataContext[key] = value;
        }

        /// <summary>
        /// Fetches the value of the given key
        /// </summary>
        /// <returns>Value or null</returns>
        public T GetData<T>(string key)
        {
            // If key in self, return
            if (this.dataContext.TryGetValue(key, out object value) && value is T t)
                return t;

            // Search in parent
            Node node = this.parent;
            while (node != null)
            {
                value = node.GetData<T>(key);
                if (value is T v)
                    return v;

                node = node.parent;
            }

            return default;
        }

        /// <summary>
        /// Removes the data associated with the given key
        /// </summary>
        /// <returns>The key was found</returns>
        public bool ClearData(string key)
        {
            Node node = this;

            while (node != null)
            {
                // If has key, remove
                if (this.dataContext.ContainsKey(key))
                {
                    this.dataContext.Remove(key);
                    return true;
                }

                // If root reached, exit
                if (node.parent == null)
                    break;

                node = node.parent;
            }

            return false;
        }

        #endregion

        #region State

        public NodeState State { get; private set; } = NodeState.NONE;

        /// <summary>
        /// Updates the state of this node
        /// </summary>
        /// <returns>New state of the node</returns>
        public NodeState Evaluate()
        {
            this.State = this.OnEvaluate();
            return this.State;
        }

        /// <summary>
        /// Called when this node gets updated
        /// </summary>
        /// <returns>State of this node</returns>
        protected abstract NodeState OnEvaluate();

        /// <summary>
        /// Resets the state of the node
        /// </summary>
        public void Reset()
        {
            this.State = NodeState.NONE;
            foreach (Node child in this)
                child.Reset();
        }

        #endregion

        #region Editor

        private string _alias = null;

        public string GetAlias() => this._alias;
        public virtual bool IsAutomaticallyHidden() => false;

        /// <summary>
        /// Shorthand to set the alias of this node
        /// </summary>
        /// <param name="alias">New alias</param>
        /// <returns>Node with the alias</returns>
        public Node Alias(string alias)
        {
            this._alias = alias;
            return this;
        }

        /// <summary>
        /// Fetches the display name of the node for the editor
        /// </summary>
        /// <returns>Text to display</returns>
        public virtual string GetText() => this.GetType().Name;

        #endregion

        #region Parent

        private Node parent;

        /// <summary>
        /// Obtains the parent of this node
        /// </summary>
        /// <returns>Parent of this node</returns>
        protected Node GetParent() => this.parent?.GetParent();

        #endregion

        #region Children

        protected readonly List<Node> children = new();

        /// <summary>
        /// Attaches the given nodes to this node
        /// </summary>
        /// <param name="nodes">Nodes to attach</param>
        public void Attach(params Node[] nodes)
        {
            foreach (Node item in nodes)
            {
                item.parent = this;
                this.children.Add(item);
            }
        }
            
        #endregion

        #region IEnumerable

        /// <inheritdoc/>
        public IEnumerator<Node> GetEnumerator() => this.children.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        #region Shorthands

        /// <summary>
        /// Shorthand for inverting a node
        /// </summary>
        /// <returns>Inverted node</returns>
        public Node Not() => new Inverter(this).Alias("NOT");

        /// <summary>
        /// Shorthand for checking if a key exists
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <returns>Exits with <see cref="NodeState.SUCCESS"/> if the key exists, otherwise exits with <see cref="NodeState.FAILURE"></returns>
        public static Node Exists<T>(string key) => new CallbackNode(
            n => n.GetData<T>(key) != null
            ? NodeState.SUCCESS
            : NodeState.FAILURE
        ).Alias($"Check for '{key}'");

        /// <summary>
        /// Shorthand for a random bool
        /// </summary>
        /// <returns>Exits with <see cref="NodeState.SUCCESS"/> 50% of the time, otherwise exits with <see cref="NodeState.FAILURE"></returns>
        public static Node RandomBool() => new CallbackNode(
            n => UnityEngine.Random.Range(0, 2) == 0
            ? NodeState.SUCCESS
            : NodeState.FAILURE
        ).Alias("Random 50%");

        #endregion
    }
}