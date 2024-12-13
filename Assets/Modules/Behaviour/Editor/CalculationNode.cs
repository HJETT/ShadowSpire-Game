using BehaviourModule.Nodes;
using System.Collections.Generic;

namespace BehaviourModule
{
    public class CalculationNode
    {
        public float x;
        public float y;
        public Node node;
        public readonly List<CalculationNode> children = new();
        public bool hideChildren;

        public static CalculationNode Create(Node node, bool allowAutomaticHide)
        {
            var calcNode = new CalculationNode { node = node };
            calcNode.hideChildren = allowAutomaticHide && node.IsAutomaticallyHidden();

            // Add children
            foreach (Node child in node)
                calcNode.children.Add(Create(child, allowAutomaticHide));

            return calcNode;
        }
    }
}