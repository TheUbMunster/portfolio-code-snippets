using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Diagnostics;
using Tao.OpenGl;
using Tao.Platform.Windows;
using NeuralNetwork;
using TBNeuralNetwork;
namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
        toptop:
        //NeuralNetworkHub.CreateNetwork("Network One").ConstructNetwork();
        top:
            //toptop:
            //    NNetworksHub nnh = new NNetworksHub();
            //    NNetworksHub.GenerateNodeDataFile(new int[] { 2, 2, 2 }, "3");
            //    //NNetworksHub.GenerateNodeDataFile(new NodeCtorData[6] { new NodeCtorData("i1", null, new double[2] { -0.75d, 0.5d }, 0.25d, new string[2] { "p1_1", "p1_2" }), new NodeCtorData("i2", null, new double[2] { 0.25d, 0.5d }, 0.25d, new string[2] { "p1_1", "p1_2" }), new NodeCtorData("p1_1", null, new double[2] { -0.75d, -0.5d }, 0.25d, new string[2] { "o1", "o2" }), new NodeCtorData("p1_2", null, new double[2] { 1d, 0.75d }, 0.25d, new string[2] { "o1", "o2" }), new NodeCtorData("o1", null, null, 0.25d, null), new NodeCtorData("o2", null, null, 0.25d, null) }, "3");
            //    NNController nn = nnh.CreateNN("test", @AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles" + @"\ND3.nnd");
            //top:
            //    Console.WriteLine("Provide inputs, or type \"New\" for a new network to be generated, or \"Export\" to export the data to the default path.");
            //    string s = Console.ReadLine();
            //    if (s.ToLower() == "new")
            //    {
            //        goto toptop;
            //    }
            //    if (s.ToLower() == "export")
            //    {
            //        nn.ExportNetworkData(@AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles" + @"\ND1");
            //        goto toptop;
            //    }
            //    else
            //    {
            //        double[] inpt = new double[2] { double.Parse(s), double.Parse(Console.ReadLine()) };
            //        NodeIOData[] nio = nn.IterateNodes(new NodeIOData[] { new NodeIOData("i1", inpt[0]), new NodeIOData("i2", inpt[1]) });
            //        foreach (NodeIOData nthing in nio)
            //        {
            //            Console.WriteLine(nthing.assocName + " " + ((int)(nthing.assocValue).Cubed().Round()).ToString());
            //        }
            //        goto top;
            //    }
        }
    }
}
namespace NeuralNetwork
{
    /// <summary>
    /// This class is one logical node. Do not interface with this directly.
    /// </summary>
    sealed class Node : INode
    {
        #region Variables
        public string NodeIdentifier { get; set; }
        //public readonly string name;
        public readonly NNController myController;
        public readonly double bias = 0;
        public readonly double[] weights;
        public readonly string[] childrenNames;
        public Node[] childrenNodes;
        public readonly List<Node> parentNodes = new List<Node>();
        private List<double> inputs = new List<double>();
        #endregion

        #region Constructor
        public Node(NodeCtorData nctrDat, out Action start)
        {
            start = Start;
            if (nctrDat.ChildWeights == null)
            {
                double[] ws = new double[nctrDat.ChildNodes.Length];
                for (int i = 0; i < ws.Length; i++)
                {
                    ws[i] = (MR.random.NextDouble() - 0.5d) * 2d;
                }
                this.weights = ws;
            }
            else
            {
                this.weights = nctrDat.ChildWeights;
            }
            if (nctrDat.Bias == null)
            {
                this.bias = 1d;
            }
            else
            {
                this.bias = nctrDat.Bias.Value;
            }
            this.NodeIdentifier = nctrDat.Name;
            this.myController = nctrDat.MyController;
            this.childrenNames = nctrDat.ChildNodes;
        }
        private void Start()
        {
            this.childrenNodes = myController.SearchNode(childrenNames);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Gets the parents of this node, if there are any.
        /// </summary>
        /// <returns>The parents of this node, if there are any.</returns>
        public INode[] GetParents()
        {
            return parentNodes.ToArray();
        }
        /// <summary>
        /// Gets the parent of this node, if there is one.
        /// </summary>
        /// <returns>The parent of this node, if there is one.</returns>
        public INode GetParent(object identifier)
        {
            return parentNodes.Find(x => x.NodeIdentifier.Equals(NodeIdentifier));
        }
        /// <summary>
        /// Gets all children of this node, if there are any.
        /// </summary>
        /// <returns>The children of this node, if there are any.</returns>
        public INode[] GetChildren()
        {
            return childrenNodes == new Node[0]? null : childrenNodes;
        }
        /// <summary>
        /// Gets a child by it's identifier.
        /// </summary>
        /// <param name="identifier">The identifier to compare against.</param>
        /// <returns>The discovered node, or null if none were found.</returns>
        public INode GetChild(object identifier)
        {
            return Array.Find<INode>(childrenNodes, x => x.NodeIdentifier.Equals(NodeIdentifier));
        }
        ///// <summary>
        ///// Gets the nodes ID cast as the given type.
        ///// </summary>
        ///// <typeparam name="T">The type to cast the ID as.</typeparam>
        ///// <returns>The ID cast as the given type.</returns>
        //public T GetID<T>() where T : class
        //{
        //    return (T)NodeIdentifier;
        //}
        /// <summary>
        /// Does the calculation iteration for this node.
        /// </summary>
        /// <param name="input">One input recieved by a parent node. (Already influenced by it's respective weight).</param>
        /// <param name="result">A reference to the result that is passed all the way down the the output nodes.</param>
        public void Calculate(double input, ref List<NodeIOData> result)
        {
            inputs.Add(input);
            if (inputs.Count >= parentNodes.Count)
            {
                input = Sigm(inputs.Sum() + bias, 3d);
                if (childrenNodes.Length != 0)
                {
                    for (int i = 0; i < childrenNodes.Length; i++)
                    {
                        double d = input * weights[i];
                        childrenNodes[i].Calculate(d, ref result);
                    }
                }
                else
                {
                    result.Add(new NodeIOData(NodeIdentifier, input));
                }
                inputs.Clear();
            }
            double Sigm(double inp, double modifier) => (((1d / (1d + Math.Pow(Math.E, -inp * Math.Abs(modifier)))) - 0.5d) * 2);
        }
        #endregion
    }
    /// <summary>
    /// This class organizes and holds one neural network. Handling NNController objects directly is not recommended. Instead, use the NNetworksHub.
    /// </summary>
    sealed class NNController
    {
        #region Variables
        public readonly string name;
        private List<Node> nodes;
        private List<NRewardData> rd = new List<NRewardData>();
        private List<Node> tempInputNodes = new List<Node>();
        private NNetworksHub myController;
        #endregion

        #region Constructor
        public NNController(NNetworksHub myController, string name)
        {
            this.name = name;
            this.myController = myController;
        }
        public NNController(NNetworksHub myController, string name, string fullDataFilePath)
        {
            this.name = name;
            this.myController = myController;
            ConstructNetwork(fullDataFilePath);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Searches this networks nodes for a node with the name "name".
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The found node.</returns>
        public Node SearchNode(string name)
        {
            if (nodes.Count == 0)
            {
                throw new System.Exception("Error: SearchNode did not find a node with the name \"" + name + "\".");
            }
            try
            {
                return nodes.First(x => x.NodeIdentifier == name);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                {
                    throw new Exception("Error: nodes list is null.");
                }
                else if (e is InvalidOperationException)
                {
                    throw new Exception("Error: SearchNode couldn't find the node \"" + name + "\" because the node list is empty or because a node of name \"" + name + "\" was not found.");
                }
            }
            return null;
        }
        /// <summary>
        /// Searches this networks nodes for all nodes with the given names.
        /// </summary>
        /// <param name="names">The names to search for.</param>
        /// <returns>The found nodes.</returns>
        public Node[] SearchNode(string[] names)
        {
            List<Node> nn = new List<Node>();
            Array.ForEach(names, x => nn.Add(SearchNode(x)));
            return nn.ToArray();
        }
        /// <summary>
        /// Iterates the node network with the provided input data.
        /// </summary>
        /// <param name="inputData">An array of the input data. Each element has an assocName, and that data will be passed to the node in the network with the same name. (If you have an input node with the name "i1", and you pass an element with an assocName of "i1", that data will be sent and processed by the "i1" node).</param>
        /// <returns>The output data for this iteration.</returns>
        public NodeIOData[] IterateNodes(NodeIOData[] inputData)
        {
            List<NodeIOData> result = new List<NodeIOData>();
            if ((inputData.Length != tempInputNodes.Count) || !tempInputNodes.All(x => 1 == inputData.Count(y => y.assocName == x.NodeIdentifier)))
            {
                tempInputNodes.Clear();
                nodes.ForEach(x => Array.ForEach(inputData, y => { if (x.NodeIdentifier == y.assocName) { tempInputNodes.Add(x); x.Calculate(y.assocValue, ref result); return; } }));
            }
            else
            {
                tempInputNodes.ForEach(x => Array.ForEach(inputData, y => { if (x.NodeIdentifier == y.assocName) { x.Calculate(y.assocValue, ref result); return; } }));
            }
            return result.ToArray();
        }
        /// <summary>
        /// Rewards a output node path tree with the given reward value. The more rewarded an output node is, the less likely that that node and its most influencial nodes will change much upon evolution.
        /// </summary>
        /// <param name="outputNodeName">The output node to reward.</param>
        /// <param name="rewardValue">The reward value to reward that node.</param>
        public void RewardNodePath(string outputNodeName, ulong rewardValue)
        {
            if (!rd.Any(x => x.assocNName == outputNodeName))
            {
                rd.Add(new NRewardData(outputNodeName, rewardValue));
            }
            else
            {
                rd.Find(x => x.assocNName == outputNodeName).AddReward(rewardValue);
            }
        }
        /// <summary>
        /// Evolves this network based on it's reward data and relative overall reward data value.
        /// </summary>
        public void EvolveNetwork([Range(0d, 1d)] double biasDeviationModifier)
        {
            List<Tuple<Node, Action>> newNodes = new List<Tuple<Node, Action>>();
            List<NRewardData> nrd = myController.GetAllRewardData();
            decimal mainMutateDeviationFactor = (((decimal)nrd.Select(x => x.TotalReward).Max()) - ((decimal)nrd.Find(x => x.assocNName == name).TotalReward)) / ((decimal)nrd.Select(x => x.TotalReward).Max());
            string newName = string.Empty;
            string[] newChildrenNodes = new string[0];
            double newBias = 0d;
            decimal thisNodeDeviationFactor = 0m;
            foreach (Node n in nodes)
            {
                //do the logic here where each node's new values are equal to their old ones influenced by the networks reward and their influence on the rewarded output nodes.
                rd.Select(x => x.TotalReward).ToList().ForEach(x => thisNodeDeviationFactor += x);
                //thisNodeDeviationFactor = rd.Find(x => x.assocNName == n.NodeIdentifier) ?? 0m / thisNodeDeviationFactor;
                newName = n.NodeIdentifier;
                newChildrenNodes = n.childrenNames;
                //newBias = n.bias;
            }



            rd.Clear();
        }
        /// <summary>
        /// Exports this networks data into a .nnd file to the specified path. (Do not include the file extension in the file path).
        /// </summary>
        /// <param name="fullOutputDataFilePath">The root data path to export to.</param>
        public void ExportNetworkData(string fullOutputDataFilePath)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            for (int i = 0; i < nodes.Count; i++)
            {
                if ((i + 1) != nodes.Count)
                {
                    //Not the last line.                                        
                    sb.Append(nodes[i].NodeIdentifier + "<" + nodes[i].bias.ToString() + ":" + string.Join(":", nodes[i].weights ?? new double[0]) + ">" + "||" + string.Join("|", nodes[i].childrenNodes.Select(x => x.NodeIdentifier) ?? new string[0]) + Environment.NewLine);
                }
                else
                {
                    //Last line.
                    sb.Append(nodes[i].NodeIdentifier + "<" + nodes[i].bias.ToString() + ":" + string.Join(":", nodes[i].weights ?? new double[0]) + ">" + "||" + string.Join("|", nodes[i].childrenNodes.Select(x => x.NodeIdentifier) ?? new string[0]));
                }
            }
            File.WriteAllText(fullOutputDataFilePath + @".nnd", sb.ToString());
        }
        /// <summary>
        /// Constructs this network using the provided data file.
        /// </summary>
        /// <param name="fullDataFilePath">The data path to the file.</param>
        public void ConstructNetwork(string fullDataFilePath)
        {
            nodes.Clear();
            tempInputNodes.Clear();
            rd.Clear();
            List<Tuple<Node, Action>> nn = new List<Tuple<Node, Action>>();
            string[] rawNodeData = File.ReadAllText(fullDataFilePath).Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            double td;
            double[] tda;
            string[] tsa;
            string[] tsss;
            string ts;
            foreach (string s in rawNodeData)
            {
                ts = s.Substring(0, s.IndexOf('<'));
                tsss = s.Substring(s.IndexOf('<') + 1, (s.IndexOf('>') - s.IndexOf('<') - 1)).Split(':');
                td = tsss[0] == "r" ? ((MR.random.NextDouble() - 0.5d) * 2d) : double.Parse(tsss[0]);
                tsss = tsss.Skip(1).ToArray();
                tda = (tsss.Length == 0 || tsss[0] == "") ? null : Array.ConvertAll(tsss, double.Parse);
                tsa = string.Join("", s.Skip(s.IndexOf('|') + 2)).Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                nn.Add(new Tuple<Node, Action>(new Node(new NodeCtorData(ts, this, tda, td, tsa), out Action a), a));
            }
            nodes = new List<Node>(nn.Select(x => x.Item1));
            foreach (Action st in nn.Select(x => x.Item2))
            {
                st?.Invoke();
            }
            nodes.ForEach(x => Array.ForEach<string>(x.childrenNodes.Select(z => z.NodeIdentifier).ToArray(), y => SearchNode(y).parentNodes.Add(x)));
        }
        #endregion
    }
    /// <summary>
    /// Use an instance of this class to create and interface with one or more neural networks.
    /// </summary>
    sealed class NNetworksHub
    {
        #region Variables
        private static NNController temp;
        private List<NNController> myNetworks = new List<NNController>();
        private List<NRewardData> rd = new List<NRewardData>();
        #endregion

        #region Constructor
        //public NNetworksHub()
        //{

        //}
        #endregion

        #region Utility
        #region GenerateNodeDataFile
        /// <summary>
        /// Generates a node data file for creating a new neural network.
        /// </summary>
        /// <param name="nodeData">The data to construct the file (the array size is equal to the total amount of nodes, including the input and output nodes). Do not use the default paramiterless struct constructor. Each node MUST have a unique name.</param>        
        public static void GenerateNodeDataFile(NodeCtorData[] nodeData)
        {
            GenerateNodeDataFile(nodeData, "");
        }
        /// <summary>
        /// Generates a node data file for creating a new neural network.
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
                    sb.Append(nodeData[i].Name + "<" + (nodeData[i].Bias.HasValue ? nodeData[i].Bias.ToString() : "r") + ":" + string.Join(":", nodeData[i].ChildWeights ?? new double[0]) + ">" + "||" + string.Join("|", nodeData[i].ChildNodes ?? new string[0]) + Environment.NewLine);
                }
                else
                {
                    //Last line.
                    sb.Append(nodeData[i].Name + "<" + (nodeData[i].Bias.HasValue ? nodeData[i].Bias.ToString() : "r") + ":" + string.Join(":", nodeData[i].ChildWeights ?? new double[0]) + ">" + "||" + string.Join("|", nodeData[i].ChildNodes ?? new string[0]));
                }
            }
            Directory.CreateDirectory(@AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles");
            File.WriteAllText(@AppDomain.CurrentDomain.BaseDirectory + @"\NodeDataFiles" + @"\ND" + @fileNumber + @".nnd", sb.ToString());
        }
        /// <summary>
        /// Generates a node data file for creating a new neural network with automatic naming conventions.
        /// </summary>
        /// <param name="nodeLayers">Element 0 is the number of input nodes, element n is the number of nodes in the nth layer, element n where n is the length of the array minus one is the number of output nodes.</param>
        public static void GenerateNodeDataFile(int[] nodeLayers)
        {
            GenerateNodeDataFile(nodeLayers, "");
        }
        /// <summary>
        /// Generates a node data file for creating a new neural network with automatic naming conventions.
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
        /// Constructs a new random neural network based on a file and gives it a name.
        /// </summary>
        /// <param name="name">The name to name your neural network.</param>
        /// <param name="fullDataFilePath">The file path of the file to construct the network off of.</param>
        /// <returns>The newly constructed neural network (It's a NNController object).</returns>
        public NNController CreateNN(string name, string fullDataFilePath)
        {
            myNetworks.ForEach(x => { if (x.name == name) { throw new Exception("Error: this NNetworksHub already contains a network with the name \"" + name + "\""); } });
            temp = new NNController(this, fullDataFilePath);
            myNetworks.Add(temp);
            return temp;
        }
        /// <summary>
        /// Accesses one NNController by name.
        /// </summary>
        /// <param name="name">The name of the NNController. you're searching for.</param>
        /// <returns>The searched NNController.</returns>
        public NNController AccessNN(string name)
        {
            temp = null;
            myNetworks.ForEach(x => { if (x.name == name) { temp = x; return; } });
            return temp;
        }
        /// <summary>
        /// Destroys a neural network by severing all of it's references to any roots.
        /// </summary>
        /// <param name="name">The name of the network to destroy.</param>
        /// <returns>True if the network was sucessfully destroyed.</returns>
        public void DestroyNN(string name)
        {
            temp = AccessNN(name);
            if (temp != null)
            {
                myNetworks.Remove(AccessNN(name));
                GC.Collect();
            }
            else
            {
                throw new KeyNotFoundException("Error: no nerual network of name \"" + name + "\" exists.");
            }
        }
        /// <summary>
        /// Gets all of the neural networks this network hub has.
        /// </summary>
        /// <returns>An array of all neural networks this hub controlls.</returns>
        public NNController[] GetAllNN()
        {
            return myNetworks.ToArray();
        }
        /// <summary>
        /// Evolves a neural network based upon its various rewards on the network and the rewards on its output nodes. If a network is not rewarded and is evolved, it will simply produce another randomly generated network.
        /// </summary>
        /// <param name="name">The name of the network to evolve.</param>
        /// <returns>The newly evolved neural network.</returns>
        public NNController EvolveNN(string name)
        {
            temp = AccessNN(name);
            //temp.EvolveNetwork();
            return temp;
        }
        /// <summary>
        /// Rewards a neural network. The more rewarded a network is, the less it evolves upon evolution.
        /// </summary>
        /// <param name="name">The name of the neural network you're rewarding.</param>
        /// <param name="rewardValue">The value you're rewarding it. (Higher is better).</param>
        public void RewardNN(string name, ulong rewardValue)
        {
            if (!rd.Any(x => x.assocNName == name))
            {
                rd.Add(new NRewardData(name, rewardValue));
            }
            else
            {
                rd.Find(x => x.assocNName == name).AddReward(rewardValue);
            }
        }
        /// <summary>
        /// Gets all of the reward data for all networks.
        /// </summary>
        /// <returns>All reward data assigned to all networks.</returns>
        public List<NRewardData> GetAllRewardData()
        {
            return rd;
        }
        #endregion
    }
    /// <summary>
    /// The data struct used to construct a new node.
    /// </summary>
    struct NodeCtorData
    {
        #region Variables
        #region Name
        /// <summary>
        /// Internal holder for Name.
        /// </summary>
        private readonly string name;
        /// <summary>
        /// My name.
        /// </summary>
        public string Name
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: Do not use the default constructor when creating a NodeCtorData object.");
                }
                return name;
            }
        }
        #endregion
        #region ChildNodes
        /// <summary>
        /// Internal holder for ChildNodes.
        /// </summary>
        private readonly string[] childNodes;
        /// <summary>
        /// The nodes I connect to; the nodes I send data to; my child nodes. (Done by node name).
        /// </summary>
        public string[] ChildNodes
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: Do not use the default constructor when creating a NodeCtorData object.");
                }
                return childNodes;
            }
        }
        #endregion
        #region ChildWeights
        /// <summary>
        /// Internal holder for ChildWeights.
        /// </summary>
        private readonly double[] childWeights;
        /// <summary>
        /// The weight values that this node has with each of it's children. (Index 0 goes to child node of index 0).
        /// </summary>
        public double[] ChildWeights
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: Do not use the default constructor when creating a NodeCtorData object.");
                }
                return childWeights;
            }
        }
        #endregion
        #region Bias
        /// <summary>
        /// Internal holder for Bias.
        /// </summary>
        private readonly double? bias;
        /// <summary>
        /// The bias of this node.
        /// </summary>
        public double? Bias
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: Do not use the default constructor when creating a NodeCtorData object.");
                }
                return bias;
            }
        }
        #endregion
        #region MyController
        /// <summary>
        /// Internal holder for MyController.
        /// </summary>
        private readonly NNController myController;
        /// <summary>
        /// This node's controller.
        /// </summary>
        public NNController MyController
        {
            get
            {
                if (!wasInitializedCorrectly)
                {
                    throw new ImproperInitializationException("Error: Do not use the default constructor when creating a NodeCtorData object.");
                }
                return myController;
            }
        }
        #endregion
        /// <summary>
        /// Is true if this object was initialized correctly.
        /// </summary>
        private readonly bool wasInitializedCorrectly;
        #endregion

        #region Constructor
        public NodeCtorData(string name, NNController myController, double[] childWeights, double? bias, string[] childNodeNames)
        {
            if ((childWeights != null && childNodeNames != null) && childWeights.Length != childNodeNames.Length)
            {
                throw new ImproperInitializationException("Error: childWeights and childNodeNames need to be the same length when a NodeCtorData object is made.");
            }
            List<string> ss = new List<string>();
            if (childNodeNames != null)
            {
                Array.ForEach(childNodeNames, x => { ss.Add(@x.Replace("|", "").Replace("<", "").Replace(">", "").Replace(":", "")); });
            }
            this.name = @name.Replace("|", "").Replace("<", "").Replace(">", "").Replace(":", "");
            this.childNodes = ss.ToArray();
            this.childWeights = childWeights;
            this.bias = bias;
            this.myController = myController;
            this.wasInitializedCorrectly = true;
        }
        #endregion
    }
    /// <summary>
    /// The data type used to input and output data from a network iteration.
    /// </summary>
    struct NodeIOData
    {
        #region Variables
        /// <summary>
        /// The name of the node this NodeIOData object is associated with.
        /// </summary>
        public readonly string assocName;
        /// <summary>
        /// The value associated.
        /// </summary>
        public readonly double assocValue;
        #endregion

        #region Constructor
        public NodeIOData(string assocName, double assocValue)
        {
            this.assocName = assocName;
            this.assocValue = assocValue;
        }
        #endregion
    }
    /// <summary>
    /// The data type used to store reward data and its associated reward recipiant.
    /// </summary>
    struct NRewardData
    {
        #region Variables
        /// <summary>
        /// The total reward that the associated object has,
        /// </summary>
        public ulong TotalReward { get; private set; }
        /// <summary>
        /// The name of the associated object.
        /// </summary>
        public readonly string assocNName;
        /// <summary>
        /// Is true if this object was initialized correctly.
        /// </summary>
        private readonly bool wasInitializedCorrectly;
        #endregion

        #region Constructors
        public NRewardData(string assocNName)
        {
            this.assocNName = assocNName;
            TotalReward = 0;
            wasInitializedCorrectly = true;
        }
        public NRewardData(string assocNName, ulong initialRewardValue)
        {
            this.assocNName = assocNName;
            TotalReward = initialRewardValue;
            wasInitializedCorrectly = true;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Adds a reward.
        /// </summary>
        /// <param name="rewardToAdd">The amount of reward to add.</param>
        public void AddReward(ulong rewardToAdd)
        {
            if (!wasInitializedCorrectly)
            {
                throw new ImproperInitializationException("Error: Do not use the default struct constructor for this object. Use an overload with at least one parameter");
            }
            TotalReward += rewardToAdd;
        }
        /// <summary>
        /// Subtracts a reward.
        /// </summary>
        /// <param name="rewardToSubtract">The amount of reward to subtract.</param>
        public void SubtractReward(ulong rewardToSubtract)
        {
            if (!wasInitializedCorrectly)
            {
                throw new ImproperInitializationException("Error: Do not use the default struct constructor for this object. Use an overload with at least one parameter");
            }
            TotalReward -= rewardToSubtract;
        }
        #endregion
    }
    /// <summary>
    /// The interface that node type objects implement.
    /// </summary>
    interface INode
    {
        /// <summary>
        /// The key that this node is identified with.
        /// </summary>
        string NodeIdentifier { get; set; }
        /// <summary>
        /// Gets the parents of this node, if there are any.
        /// </summary>
        /// <returns>The parents of this node, if there are any.</returns>
        INode[] GetParents();
        /// <summary>
        /// Gets the parent of this node, if there is one.
        /// </summary>
        /// <returns>The parent of this node, if there is one.</returns>
        INode GetParent(object identifier);
        /// <summary>
        /// Gets all children of this node, if there are any.
        /// </summary>
        /// <returns>The children of this node, if there are any.</returns>
        INode[] GetChildren();
        /// <summary>
        /// Gets a child by it's identifier.
        /// </summary>
        /// <param name="identifier">The identifier to compare against.</param>
        /// <returns>The discovered node, or null if none were found.</returns>
        INode GetChild(object identifier);
        ///// <summary>
        ///// Gets the nodes ID cast as the given type.
        ///// </summary>
        ///// <typeparam name="T">The type to cast the ID as.</typeparam>
        ///// <returns>The ID cast as the given type.</returns>
        //T GetID<T>() where T : class;
    }
    //interface INewINode<TKey>
    //{        
    //    TKey NodeIdentifier { get; set; }
    //    INewINode<TKey>[] GetChildren();
    //    INewINode<TKey> GetChild(TKey key);
    //    INewINode<TKey>[] GetParents();
    //    INewINode<TKey> GetParent(TKey key);
    //}
    /// <summary>
    /// The exception that is thrown when an attempt to operate on an object that was not initialized correctly is made OR when an object is not initialized correctly.
    /// </summary>
    [Serializable]
    public sealed class ImproperInitializationException : Exception
    {
        public ImproperInitializationException()
        {

        }
        public ImproperInitializationException(string message) : base(message)
        {

        }
        public ImproperInitializationException(string message, Exception inner) : base(message, inner)
        {

        }
        protected ImproperInitializationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }
    /// <summary>
    /// The class that holds the master random object.
    /// </summary>
    static class MR
    {
        /// <summary>
        /// The Master Random.
        /// </summary>
        public static Random random = new Random();
    }
}