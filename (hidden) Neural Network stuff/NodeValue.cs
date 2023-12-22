using System;
using System.Collections.Generic;
using System.Text;

namespace TBNeuralNetwork
{
    /// <summary>
    /// Contains a node/value pair.
    /// </summary>
    public struct NodeValue
    {
        #region Variables
        #region AssocKey
        /// <summary>
        /// Internal AssocNodeKey
        /// </summary>
        private object assocNodeKey;
        /// <summary>
        /// The associated node's key.
        /// </summary>
        public object AssocNodeKey
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocNodeKey;
            }
            set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocNodeKey = value;
            }
        }
        #endregion
        #region AssocValue
        /// <summary>
        /// Internal AssocValue
        /// </summary>
        private object assocValue;
        /// <summary>
        /// The associated node's value.
        /// </summary>
        public object AssocValue
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                return assocValue;
            }
            set
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: this object was initialized incorrectly. Use the paramiterized constructor.");
                }
                assocValue = value;
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
        /// The constructor for creating an associated node/associated value pair.
        /// </summary>
        /// <param name="assocNodeKey">The key for the associated node.</param>
        /// <param name="assocValue">The value for the associated node.</param>
        public NodeValue(object assocNodeKey, object assocValue)
        {
            this.assocNodeKey = assocNodeKey;
            this.assocValue = assocValue;
            wasInitializedCorrectly = true;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Gets the node key cast as the given type.
        /// </summary>
        /// <typeparam name="T">The type to cast the key as.</typeparam>
        /// <returns>The key cast as the given type.</returns>
        public T GetNodeKey<T>()
        {
            return (T)AssocNodeKey;
        }
        /// <summary>
        /// Gets the value cast as the given type.
        /// </summary>
        /// <typeparam name="T">The type to cast the value as.</typeparam>
        /// <returns>The value cast as the given type.</returns>
        public T GetValue<T>()
        {
            return (T)AssocValue;
        }
        #endregion
    }
}
