using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TBNeuralNetwork
{
    /// <summary>
    /// A neural network node.
    /// </summary>
    public class Node /*: INode*/
    {
        #region Interface Members
        /// <summary>
        /// The object key used to identify this node. This key should be unique among the network.
        /// </summary>
        public object NodeIdentifier { get; private set; }
        /// <summary>
        /// The array that stores this node's children.
        /// </summary>
        public INode[] ChildrenNodes { get; private set; }
        /// <summary>
        /// The array that stores this node's parents.
        /// </summary>
        public INode[] ParentNodes { get; private set; }
        /// <summary>
        /// Stores all nodes in this network.
        /// </summary>
        public INode AllNodes { get; set; }
        /// <summary>
        /// Gets all children of this node.
        /// </summary>
        /// <returns>An array of this node's children, if any were found.</returns>
        public INode[] GetChildren()
        {
            return ChildrenNodes;
        }
        /// <summary>
        /// Gets a child of this node by its identifier key.
        /// </summary>
        /// <param name="identifier">The key to compare against each NodeIdentifier.</param>
        /// <returns>The dicovered node, if one was found.</returns>
        public INode GetChild(object identifier)
        {
            return Array.Find(ChildrenNodes, x => x.NodeIdentifier == identifier);
        }
        /// <summary>
        /// Gets all parents of this node.
        /// </summary>
        /// <returns>An array of this node's parents, if any were found.</returns>
        public INode[] GetParents()
        {
            return ParentNodes;
        }
        /// <summary>
        /// Gets a parent of this node by its identifier key.
        /// </summary>
        /// <param name="identifier">The key to compare against each NodeIdentifier.</param>
        /// <returns>The dicovered node, if one was found.</returns>
        public INode GetParent(object identifier)
        {
            return Array.Find(ParentNodes, x => x.NodeIdentifier == identifier);
        }
        /// <summary>
        /// The function used to calculate this node's processing iteration.
        /// </summary>
        /// <param name="input">An individual input recieved from a parent by this node.</param>
        /// <param name="result">A reference to the result; passed down to the output nodes.</param>
        public void Operate(object input, ref Stack<object> result)
        {
            double Sigm(double inp, double modifier) => (((1d / (1d + Math.Pow(Math.E, -inp * Math.Abs(modifier)))) - 0.5d) * 2);
            inputs.Push(((double)input));
            if (inputs.Count < ParentNodes.Length)
            {
                return;
            }
            else
            {
                input = Sigm(inputs.Sum() + bias, 3d);
                inputs.Clear();
                if (ChildrenNodes.Length == 0)
                {
                    for (int i = 0; i < ChildrenNodes.Length; i++)
                    {
                        ChildrenNodes[i].Operate(((double)input) * weights[i], ref result);
                    }
                }
                else
                {
                    result.Push(new NodeValue(NodeIdentifier, input));
                }
            }
        }
        #endregion

        #region Other Variables
        /// <summary>
        /// The names of this node's children nodes.
        /// </summary>
        private readonly object[] childKeys;
        /// <summary>
        /// The names of this node's parent nodes.
        /// </summary>
        private readonly object[] parentKeys;
        /// <summary>
        /// The bias of this node.
        /// </summary>
        [Range(-1, 1)] public readonly double bias;
        /// <summary>
        /// The weight influences for this nodes children.
        /// </summary>
        [Range(-1, 1)] public readonly double[] weights;
        private Stack<double> inputs = new Stack<double>();
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor used to assemble a new neural network node.
        /// </summary>
        /// <param name="nCtorD">The constructor data used to create a new node.</param>
        /// <param name="init">The initialization method that must be called AFTER ALL nodes have been created.</param>
        public Node(NodeCtorData nCtorD, out Action<INode> init)
        {
            NodeIdentifier = nCtorD.AssocNodeID;
            childKeys = nCtorD.AssocChildNodes;
            parentKeys = nCtorD.AssocParentNodes;
            bias = nCtorD.AssocNodeBias == null ? (NeuralNetworkHub.MasterRandom.NextDouble() - 0.5d) * 2d : nCtorD.AssocNodeBias.Value;
            weights = nCtorD.AssocNodeWeights;
            init = InitializeNodeConnections;
        }
        /// <summary>
        /// The method that initializes the node connections using the node names.
        /// </summary>
        public void InitializeNodeConnections(INode allNodes)
        {
            AllNodes = allNodes;
            ChildrenNodes = Array.ConvertAll(childKeys, x => GetChild(x));
            ParentNodes = Array.ConvertAll(parentKeys, x => GetChild(x));
        }
        #endregion
    }
}