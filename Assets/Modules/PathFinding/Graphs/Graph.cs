using System;
using System.Collections.Generic;
using PathFinding.Nodes;

namespace PathFinding.Graphs
{
    public abstract class Graph<T> where T : Node
    {
        #region Nodes

        /// <summary>
        /// ID of the node (KEY) with the node itself (VALUE)
        /// </summary>
        protected readonly Dictionary<int, T> _nodes = new();

        public T GetNode(int id)
        {
            if (!_nodes.TryGetValue(id, out var node))
                throw new ArgumentOutOfRangeException($"No node was found with the id '{id}'.");
            return node;
        }

        #endregion

        #region Link

        /// <summary>
        /// Creates a link from a node to another one
        /// </summary>
        /// <param name="from">Index of the node to start from</param>
        /// <param name="to">Index of the node to end at</param>
        /// <param name="cost">Cost of the path</param>
        /// <param name="isBidirectional">Determines if the link is bidirectional</param>
        public void AddLink(int from, int to, float cost, bool isBidirectional)
        {
            // If same points, error
            if (from == to)
                throw new ArgumentException("Cannot link a node to itself.");

            // If key doesn't exist, error
            if (!_nodes.TryGetValue(from, out var node))
                throw new ArgumentException("Cannot link to a node that doesn't exist.");

            node.AddLink(to, cost);

            // If bidirectional, add reverse
            if (isBidirectional)
                AddLink(to, from, cost, false);
        }

        #endregion

        #region Algorithm

        /// <summary>
        /// Finds a path from the given node to the other given node
        /// </summary>
        /// <param name="start">Start of the path</param>
        /// <param name="end">End of the path</param>
        /// <returns>Path found or null if not found</returns>
        public int[] GetPath(int start, int end)
        {
            Dictionary<int, int> links = this.ComputePath(start, end);

            if (links == null)
                return null;

            // If not path found, skip
            if (!links.ContainsKey(end))
                return null;

            // Compile path
            var path = new List<int>();

            var currentNode = end;
            do
            {
                currentNode = links[currentNode];
                path.Add(currentNode);
            } while (currentNode != -1);

            path.RemoveAt(path.Count - 1); // Remove last (-1)
            path.Reverse(); // Reverse the order (start -> end)
            path.Add(end); // Add end node
            return path.ToArray();
        }

        protected abstract float GetHeuristic(int start, int end);

        /// <summary>
        /// Finds the path between the two given nodes
        /// </summary>
        /// <returns>Links from nodes to nodes</returns>
        protected virtual Dictionary<int, int> ComputePath(int start, int end)
        {
            var links = new Dictionary<int, int>
            {
                [start] = -1 // Comes from none
            };

            var costPath = new Dictionary<int, float>
            {
                [start] = 0 // Cost nothing
            };

            var frontier = new UniquePriorityQueue<Node, float>();
            frontier.Enqueue(GetNode(start), 0); // Add start

            do
            {
                var current = frontier.Dequeue();

                if (current.Id == end)
                    break;

                foreach (var next in current.GetNeighbors())
                {
                    var newCost = costPath[current.Id] + current.GetCost(next);

                    // If cost higher, skip
                    if (costPath.TryGetValue(next, out var oldCost) && newCost >= oldCost)
                        continue;

                    costPath[next] = newCost;
                    links[next] = current.Id;

                    // Continue this path
                    frontier.Enqueue(GetNode(next), newCost + GetHeuristic(next, end));
                }
            } while (frontier.Count > 0);

            return links;
        }

        #endregion
    }
}