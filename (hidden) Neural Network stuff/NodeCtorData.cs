using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TBNeuralNetwork
{
    /// <summary>
    /// The constructor object for a node.
    /// </summary>
    public struct NodeCtorData
    {
        #region Variables
        #region AssocNodeID
        /// <summary>
        /// Internal AssocNodeID.
        /// </summary>
        private object assocNodeID;
        /// <summary>
        /// The associated NodeIdentifier.
        /// </summary>        
        public object AssocNodeID
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocNodeID;
            }
            private set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocNodeID = value;
            }
        }
        #endregion
        #region AssocChildNodes
        /// <summary>
        /// Internal AssocChildNodes.
        /// </summary>
        private object[] assocChildNodes;
        /// <summary>
        /// The associated nodes children nodes.
        /// </summary>        
        public object[] AssocChildNodes
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocChildNodes;
            }
            private set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocChildNodes = value;
            }
        }
        #endregion
        #region AssocParentNodes
        /// <summary>
        /// Internal AssocParentNodes.
        /// </summary>
        private object[] assocParentNodes;
        /// <summary>
        /// The associated nodes parent nodes.
        /// </summary>          
        public object[] AssocParentNodes
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocParentNodes;
            }
            private set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocParentNodes = value;
            }
        }
        #endregion
        #region AssocNodeBias
        /// <summary>
        /// Internal AssocNodeID.
        /// </summary>
        [Range(-1 , 1)] private double? assocNodeBias;
        /// <summary>
        /// The associated NodeIdentifier.
        /// </summary>        
        [Range(-1, 1)] public double? AssocNodeBias
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocNodeBias;
            }
            private set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocNodeBias = value;
            }
        }
        #endregion
        #region AssocNodeWeights
        /// <summary>
        /// Internal AssocNodeID.
        /// </summary>
        [Range(-1, 1)] private double[] assocNodeWeights;
        /// <summary>
        /// The associated NodeIdentifier.
        /// </summary>        
        [Range(-1, 1)] public double[] AssocNodeWeights
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocNodeWeights;
            }
            private set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocNodeWeights = value;
            }
        }
        #endregion
        /// <summary>
        /// Is set to true in the paramiterized constructor if the object was initialized correctly.
        /// </summary>
        private bool wasInitializedCorrectly;
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor used to assemble a NodeCtorData object.
        /// </summary>
        /// <param name="nodeIdentifier">The node's identifier.</param>
        /// <param name="nodeBias">The node's bias.</param>
        /// <param name="nodeWeights">The node's children assigned weights.</param>
        /// <param name="childNodeIdentifiers">The node's childrens identifiers.</param>
        /// <param name="parentNodeIdentifiers">The node's parents identifiers.</param>
        public NodeCtorData(object nodeIdentifier, [Range(-1, 1)] double? nodeBias, double[] nodeWeights, object[] childNodeIdentifiers, object[] parentNodeIdentifiers)
        {
            assocNodeID = nodeIdentifier;
            assocChildNodes = childNodeIdentifiers;
            assocParentNodes = parentNodeIdentifiers;
            assocNodeBias = nodeBias;
            assocNodeWeights = nodeWeights;
            wasInitializedCorrectly = true;
        }
        #endregion
    }
}
