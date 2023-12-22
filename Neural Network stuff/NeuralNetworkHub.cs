using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TBNeuralNetwork
{
    public static class NeuralNetworkHub
    {
        #region Variables
        /// <summary>
        /// This NeuralNetworkHub's NeuralNetworkControllers.
        /// </summary>
        private static List<NeuralNetworkController> myControllers = new List<NeuralNetworkController>();
        /// <summary>
        /// A temporary holder variable used in various functions
        /// </summary>
        private static NeuralNetworkController temp;
        /// <summary>
        /// The master random instance.
        /// </summary>
        public static Random MasterRandom { get; private set; } = new Random();
        #endregion

        #region Utility
        #region GenerateNodeDataFile
        /// <summary>
        /// Generates a nnd file for creating a new neural network.
        /// </summary>
        /// <param name="nodeData">The data to construct the file (the array size is equal to the total amount of nodes, including the input and output nodes). Do not use the default paramiterless struct constructor. Each node MUST have a unique name.</param>        
        public static void GenerateNodeDataFile(NodeCtorData[] nodeData)
        {
            GenerateNodeDataFile(nodeData, "");
        }
        /// <summary>
        /// Generates a nnd file for creating a new neural network.
        /// </summary>
        /// <param name="nodeData">The data to construct the file (the array size is equal to the total amount of nodes, including the input and output nodes). Do not use the default paramiterless struct constructor. Each node MUST have a unique name.</param>
        /// <param name="fileNumber">An additional number to add to the filename, so you can have multiple Node Data files.</param>
        public static void GenerateNodeDataFile(NodeCtorData[] nodeData, string fileNumber)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            for (int i = 0; i < nodeData.Length; i++)
            {
                if ((i + 1) != nodeData.Length)
                {
                    //Not the last line.                                        
                    sb.Append(nodeData[i].AssocNodeID + "<" + (nodeData[i].AssocNodeBias.HasValue ? nodeData[i].AssocNodeBias.ToString() : "r") + ":" + string.Join(":", nodeData[i].AssocNodeWeights ?? new double[0]) + ">" + "||" + string.Join("|", nodeData[i].AssocChildNodes ?? new string[0]) + Environment.NewLine);
                }
                else
                {
                    //Last line.
                    sb.Append(nodeData[i].AssocNodeID + "<" + (nodeData[i].AssocNodeBias.HasValue ? nodeData[i].AssocNodeBias.ToString() : "r") + ":" + string.Join(":", nodeData[i].AssocNodeWeights ?? new double[0]) + ">" + "||" + string.Join("|", nodeData[i].AssocChildNodes ?? new string[0]));
                }
            }
            Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles");
            File.WriteAllText(@AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles" + @"\ND" + @fileNumber + @".nnd", sb.ToString());
        }
        /// <summary>
        /// Generates a nnd file for creating a new neural network with automatic naming conventions and random paramiters.
        /// </summary>
        /// <param name="nodeLayers">Element 0 is the number of input nodes, element n is the number of nodes in the nth layer, element n where n is the length of the array minus one is the number of output nodes.</param>
        public static void GenerateNodeDataFile(int[] nodeLayers)
        {
            GenerateNodeDataFile(nodeLayers, "");
        }
        /// <summary>
        /// Generates a nnd file for creating a new neural network with automatic naming conventions and random paramiters.
        /// </summary>
        /// <param name="nodeLayers">Element 0 is the number of input nodes, element n is the number of nodes in the nth layer, element n where n is the length of the array minus one is the number of output nodes.</param>
        /// <param name="fileNumber">An additional number to add to the filename, so you can have multiple Node Data files.</param>
        public static void GenerateNodeDataFile(int[] nodeLayers, string fileNumber)
        {
            List<NodeCtorData> ncd = new List<NodeCtorData>();
            string[] cn = new string[] { string.Empty };
            for (int i = 0; i < nodeLayers.Length; i++)
            {
                for (int j = 0; j < nodeLayers[i]; j++)
                {
                    if ((i + 1) < nodeLayers.Length)
                    {
                        cn = new string[nodeLayers[i + 1]];
                        if (i < nodeLayers.Length - 2)
                        {
                            for (int k = 0; k < nodeLayers[i + 1]; k++)
                            {
                                cn[k] = "p" + (i + 1).ToString() + "_" + (k + 1).ToString();
                            }
                        }
                        else
                        {
                            for (int k = 0; k < nodeLayers[i + 1]; k++)
                            {
                                cn[k] = "o" + (k + 1).ToString();
                            }
                        }
                    }
                    else
                    {
                        cn = new string[] { string.Empty };
                    }
                    if (i == 0)
                    {
                        ncd.Add(new NodeCtorData("i" + (j + 1).ToString(), null, null, null, cn));
                    }
                    else if ((i + 1) < nodeLayers.Length)
                    {
                        ncd.Add(new NodeCtorData("p" + (i).ToString() + "_" + (j + 1).ToString(), null, null, null, cn));
                    }
                    else
                    {
                        ncd.Add(new NodeCtorData("o" + (j + 1).ToString(), null, null, null, cn));
                    }
                }
            }
            GenerateNodeDataFile(ncd.ToArray(), fileNumber);
        }
        #endregion
        /// <summary>
        /// Creates a new network and adds it to the hub.
        /// </summary>
        /// <param name="networkIdentifier">The identifier to assign to the new network.</param>
        /// <returns>The newly created network.</returns>
        public static NeuralNetworkController CreateNetwork(object networkIdentifier)
        {
            temp = new NeuralNetworkController(networkIdentifier);
            myControllers.Add(temp);
            return temp;
        }
        #endregion
    }
}
