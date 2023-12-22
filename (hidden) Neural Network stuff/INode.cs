using System;
using System.Collections.Generic;

namespace TBNeuralNetwork
{
    /// <summary>
    /// The interface that node objects implement.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// The object key used to identify this node. This key should be unique among the network.
        /// </summary>
        object NodeIdentifier { get; set; }
        /// <summary>
        /// The array that stores this node's children.
        /// </summary>
        INode[] ChildrenNodes { get; set; }
        /// <summary>
        /// The array that stores this node's parents.
        /// </summary>
        INode[] ParentNodes { get; set; }
        /// <summary>
        /// Stores all nodes in this network.
        /// </summary>
        INode AllNodes { get; set; }
        /// <summary>
        /// Gets all children of this node.
        /// </summary>
        /// <returns>An array of this node's children, if any were found.</returns>
        INode[] GetChildren();
        /// <summary>
        /// Gets a child of this node by its identifier key.
        /// </summary>
        /// <param name="identifier">The key to compare against each NodeIdentifier.</param>
        /// <returns>The dicovered node, if one was found.</returns>
        INode GetChild(object identifier);
        /// <summary>
        /// Gets all parents of this node.
        /// </summary>
        /// <returns>An array of this node's parents, if any were found.</returns>
        INode[] GetParents();
        /// <summary>
        /// Gets a parent of this node by its identifier key.
        /// </summary>
        /// <param name="identifier">The key to compare against each NodeIdentifier.</param>
        /// <returns>The dicovered node, if one was found.</returns>
        INode GetParent(object identifier);
        /// <summary>
        /// Recieves data individually from the parent nodes and operates on it. Pushes the data on the result stack reference if this node is an output node, otherwise it passes it down to the next node.
        /// </summary>
        /// <param name="data">The individual data object recieved by a parent.</param>
        /// <param name="result">A reference to the result object passed down to the output nodes.</param>        
        void Operate(object data, ref Stack<object> result);
    }
}
