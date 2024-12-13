using BehaviourModule.Interfaces;
using BehaviourModule.Nodes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaviourModule
{
    public class TreeVisualizer : EditorWindow
    {
        [MenuItem("Window/Tree Visualizer")]
        public static void ShowWindow() => GetWindow<TreeVisualizer>("Tree Visualizer");

        public void Update()
        {
            if (Application.isPlaying)
                this.Repaint();
        }

        private void OnGUI()
        {
            this.DrawOptions();
            this.DrawVisualizer();
        }

        private void OnSelectionChange() => this.Repaint();

        private bool useTypeName;
        private bool recalculateOnHide = true;
        private bool allowAutomaticHide = true;

        private bool useSelection = true;
        private List<GameObject> trees = new();
        private int treeSelected = -1;

        private bool useSquaredLinks = true;

        #region Current Tree

        private IVisualizable currentTree;
        private CalculationNode root;

        private void CheckForNew(GameObject target)
        {
            // If invalid, skip
            if (target == null)
            {
                this.currentTree = null;
                return;
            }

            // If no tree, skip
            if (!target.TryGetComponent(out IVisualizable tree))
                return;

            // If same tree, skip
            if (this.currentTree?.GetRoot() != null && tree == this.currentTree)
                return;

            this.currentTree = tree;

            // Create if not created
            Node rootNode = this.currentTree.GetRoot();

            if (rootNode == null)
            {
                this.currentTree.RebuildRoot();
                rootNode = this.currentTree.GetRoot();
            }

            // Set up tree
            this.root = CalculationNode.Create(rootNode, this.allowAutomaticHide);
            this.BuildFromRoot();
        }

        private void BuildFromRoot()
        {
            // Reset values
            this.minPos = this.maxPos = Vector2.zero;

            // Set up tree
            this.CalculatePositions(this.root, 0, 0);
        }

        #endregion

        #region Calculation Node

        private Vector2 minPos;
        private Vector2 maxPos;
        private Vector3 posOffset;

        private float CalculatePositions(CalculationNode node, float _x, float _y)
        {
            float offset = 0f;

            // Calculate positions for children
            if (!node.hideChildren)
            {
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (CalculationNode t in node.children)
                {
                    offset += this.CalculatePositions(
                        t,
                        _x + offset,
                        _y + 1
                    );
                }
            }

            if (offset <= 0)
                offset = 1;

            node.x = _x + ((offset - 1) / 2f);
            node.y = _y;

            if (this.minPos.x > node.x)
                this.minPos.x = node.x;

            if (this.maxPos.x < node.x)
                this.maxPos.x = node.x;

            if (this.maxPos.y < node.y)
                this.maxPos.y = node.y;

            return offset;
        }

        #endregion

        #region Visualizer

        private const float NODE_WIDTH = 100f;
        private const float NODE_HEIGHT = 50f;
        private const float NODE_MARGIN = 25f;

        /// <summary>
        /// Fetches the text to display for a node
        /// </summary>
        /// <param name="calcNode">Node to display</param>
        /// <returns>Text to display</returns>
        private string GetNodeText(CalculationNode calcNode)
        {
            Node node = calcNode.node;

            // Prevent crashes
            if (node is null)
                return "NULL";

            if (this.useTypeName)
                return node.GetType().Name;

            return node.GetAlias() ?? node.GetText();
        }

        /// <summary>
        /// Fetches the canvas position of a node at the given position
        /// </summary>
        /// <param name="x">X position of the node</param>
        /// <param name="y">Y position of the node</param>
        /// <returns>Canvas position of the node</returns>
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private Vector3 GetNodePosition(float x, float y) => new Vector3()
        {
            x = (x * (NODE_WIDTH + NODE_MARGIN)) + NODE_MARGIN,
            y = (y * (NODE_HEIGHT + NODE_MARGIN)) + NODE_MARGIN
        } + this.posOffset;

        /// <summary>
        /// Fetches the color of the given node
        /// </summary>
        /// <param name="node">Node to analyze</param>
        /// <returns>Color of the node</returns>
        private Color GetNodeColor(Node node) => node?.State switch
        {
            NodeState.FAILURE => Color.red,
            NodeState.RUNNING => Color.yellow,
            NodeState.SUCCESS => Color.green,
            _ => Color.white
        };

        #endregion

        #region Draw

        private GUIStyle nodeStyle;
        private GUIStyle visualizerStyle;
        private Vector2 visualizerScrollPos;

        /// <summary>
        /// Initializes the styles for the editor
        /// </summary>
        private void InitializeStyles()
        {
            this.nodeStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
            };

            this.visualizerStyle = new GUIStyle(GUI.skin.box)
            {
                normal = new GUIStyleState
                {
                    background = Texture2D.whiteTexture, // Fallback, will be overwritten
                    textColor = Color.white
                },
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            // Set the dark background color
            var bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.15f, 1f)); // Dark gray background
            bgTexture.Apply();
            this.visualizerStyle.normal.background = bgTexture;
        }

        /// <summary>
        /// Draws an arrow between the two given nodes
        /// </summary>
        /// <param name="parent">Node to start the arrow from</param>
        /// <param name="child">Node to end the arrow at</param>
        private void DrawArrowBetweenNodes(CalculationNode parent, CalculationNode child)
        {
            Vector3 pos1 = this.GetNodePosition(parent.x, parent.y) + new Vector3(NODE_WIDTH / 2f, NODE_HEIGHT);
            Vector3 pos2 = this.GetNodePosition(child.x, child.y) + new Vector3(NODE_WIDTH / 2f, 0);

            Handles.color = this.GetNodeColor(child.node);

            List<Vector3> positions = new();

            if (this.useSquaredLinks)
            {
                float middleY = pos1.y + (NODE_MARGIN / 2f);

                positions.Add(pos1);
                positions.Add(new Vector3(pos1.x, middleY));
                positions.Add(new Vector3(pos2.x, middleY));
                positions.Add(pos2);
            }
            else
            {
                positions.Add(pos1);
                positions.Add(pos2);
            }

            for (int i = 1; i < positions.Count; i++)
            {
                if (child.hideChildren)
                    Handles.DrawDottedLine(positions[i - 1], positions[i], 0.2f);
                else
                    Handles.DrawLine(positions[i - 1], positions[i]);
            }
        }

        /// <summary>
        /// Draws the given node with its children
        /// </summary>
        /// <param name="parent">Node to draw</param>
        private void DrawWithChildren(CalculationNode parent)
        {
            // Skip if invalid
            if (parent is null)
                return;

            // Draw self
            this.DrawSelf(parent);

            if (parent.hideChildren)
                return;

            // Display recursively
            foreach (CalculationNode child in parent.children)
            {
                // Draw arrow from parent to child
                this.DrawArrowBetweenNodes(parent, child);

                // Draw children
                this.DrawWithChildren(child);
            }
        }

        /// <summary>
        /// Draws the given node
        /// </summary>
        /// <param name="self">Node to draw</param>
        private void DrawSelf(CalculationNode self)
        {
            Node node = self.node;
            float x = self.x;
            float y = self.y;

            // Skip if node is invalid
            if (node is null)
                return;

            string text = this.GetNodeText(self);

            // Stylize the node
            if (this.nodeStyle is null)
                return;

            this.nodeStyle.normal.textColor = this.GetNodeColor(node);

            // Position the node
            var rect = new Rect { position = this.GetNodePosition(x, y), width = NODE_WIDTH, height = NODE_HEIGHT };

            GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

            if (GUI.Button(rect, text, this.nodeStyle) && self.children.Count > 0)
            {
                self.hideChildren = !self.hideChildren;

                if (this.recalculateOnHide)
                    this.BuildFromRoot();
            }
        }

        /// <summary>
        /// Draws the tree visualizer
        /// </summary>
        private void DrawVisualizer()
        {
            bool hasTree = this.currentTree != null;

            this.InitializeStyles();

            float height = this.position.height;

            var scrollViewRect = new Rect(0, height * 0.25f, this.position.width, height * 0.75f);

            GUI.Box(scrollViewRect, GUIContent.none, this.visualizerStyle);

            Vector3 viewSize = scrollViewRect.size;

            if (hasTree)
            {
                Vector3 max = this.GetNodePosition(this.maxPos.x + 1, this.maxPos.y);
                Vector3 min = this.GetNodePosition(this.minPos.x, this.minPos.y);
                min.y = Mathf.Abs(min.y) + NODE_MARGIN * 2;

                viewSize = max + min;
            }

            this.visualizerScrollPos = GUI.BeginScrollView(
                scrollViewRect,
                this.visualizerScrollPos,
                new Rect(0, 0, viewSize.x, viewSize.y)
            );

            if (hasTree)
            {
                this.DrawWithChildren(this.root);
            }
            else
            {
                GUI.Label(new Rect(0, 0, viewSize.x, viewSize.y), "No Tree Selected", new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = {
                        textColor = new Color(1f, 0.4f, 0.7f)
                    }
                });
            }

            GUI.EndScrollView();
        }

        /// <summary>
        /// Draws the options of the visualizer
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.Space(5f);
            GUILayout.Label(new GUIContent(
                "Visual",
                "Options that only change the apparence of the visualizer"
            ), EditorStyles.boldLabel);

            this.useTypeName = GUILayout.Toggle(this.useTypeName, new GUIContent(
                "Use Types",
                "Shows the type of the node instead of their display name"
            ));

            this.useSquaredLinks = GUILayout.Toggle(this.useSquaredLinks, new GUIContent(
                "Use Squared Links",
                "Makes the line between nodes square to ease the reading"
            ));

            this.allowAutomaticHide = GUILayout.Toggle(this.allowAutomaticHide, new GUIContent(
                "Allow Automatic Hide",
                "Allows nodes to automatically hide themselves when the tree is first loaded"
            ));

            GUILayout.Space(5f);
            GUILayout.Label(new GUIContent(
                "Compute",
                "Options that change how to the visualizer behaves"
            ), EditorStyles.boldLabel);

            this.recalculateOnHide = GUILayout.Toggle(this.recalculateOnHide, new GUIContent(
                "Recalculate on hide",
                "Recalculates the visualizer when a node is toggled"
            ));

            this.useSelection = GUILayout.Toggle(this.useSelection, new GUIContent(
                "Use selection",
                "Shows the tree selected in the Hierarchy"
            ));

            if (!this.useSelection)
            {
                GUILayout.BeginHorizontal();

                this.treeSelected = EditorGUILayout.Popup("Tree selection", this.treeSelected, this.trees.Select(o => o.name).ToArray());

                if (GUILayout.Button(new GUIContent("Scan", "Fetches all the trees in the scene"), GUILayout.Width(50)))
                {
                    GameObject cur = null;

                    if (this.treeSelected >= 0 && this.treeSelected < this.trees.Count)
                        cur = this.trees[this.treeSelected];

                    this.trees = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID)
                        .Where(t => t is IVisualizable)
                        .Select(t => t.gameObject)
                        .ToList();
                    this.treeSelected = this.trees.FindIndex(c => c == cur);
                }

                GUILayout.EndHorizontal();

                if (this.treeSelected < 0 || this.treeSelected >= this.trees.Count)
                    return;

                this.CheckForNew(this.trees[this.treeSelected]);
            }
            else
            {
                this.CheckForNew(Selection.activeGameObject);
            }
        }

        #endregion
    }
}