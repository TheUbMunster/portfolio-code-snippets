using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace TBNeuralNetwork
{
    /// <summary>
    /// A neural network controller.
    /// </summary>
    public class NeuralNetworkController
    {
        #region Variables
        /// <summary>
        /// This NeuralNetworkControllers identifier.
        /// </summary>
        public object NNControllerIdentifier { get; private set; }
        /// <summary>
        /// This NeuralNetworkControllers nodes.
        /// </summary>
        public Node[] nodes;
        /// <summary>
        /// The array that holds the input nodes as they currently stand.
        /// </summary>
        public Node[] tempInputNodes;
        private NodeValue rewardData;
        #endregion

        #region Constructor
        /// <summary>
        /// The constructor used to create a new NeuralNetworkController.
        /// </summary>
        /// <param name="identifier">The identifier to assign to the new NeuralNetworkController. This ID should be unique among the other networks in the parent hub.</param>
        /// <param name="myHub">The parent hub; the hub that controlls this network.</param>
        public NeuralNetworkController(object identifier)
        {
            NNControllerIdentifier = identifier;
            rewardData = new NodeValue(identifier, 0d);            
        }
        #endregion

        #region Utility
        /// <summary>
        /// Iterates this neural network with the given input data.
        /// </summary>
        /// <param name="inputData">The input data to provide to the network.</param>
        /// <returns>The output data of this networks iteration.</returns>
        public NodeValue[] IterateNetwork(NodeValue[] inputData)
        {
            Stack<object> result = new Stack<object>();
            Array.ForEach(nodes, x => Array.ForEach(inputData, y => { if (x.NodeIdentifier == y.AssocNodeKey) { x.Operate((double)y.AssocValue, ref result); } }));
            return Array.ConvertAll<object, NodeValue>(result.ToArray(), x => (NodeValue)x);
        }
        /// <summary>
        /// Constructs this network using the provided data file.
        /// </summary>
        /// <param name="nndFilePath">The data path to the nnd data file.</param>
        public void ConstructNetwork(string nndFilePath)
        {            
            rewardData = new NodeValue(NNControllerIdentifier, 0d);
            List<Tuple<Node, Action<INode>>> nn = new List<Tuple<Node, Action<INode>>>();
            string[] rawNodeData = File.ReadAllText(nndFilePath).Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            nodes = new Node[rawNodeData.Length];
            tempInputNodes = null;
            double td;
            double[] tda;
            string[] tsa;
            string[] tsss;
            string ts;
            foreach (string s in rawNodeData)
            {
                ts = s.Substring(0, s.IndexOf('<'));
                tsss = s.Substring(s.IndexOf('<') + 1, (s.IndexOf('>') - s.IndexOf('<') - 1)).Split(':');
                td = tsss[0] == "r" ? ((NeuralNetworkHub.MasterRandom.NextDouble() - 0.5d) * 2d) : double.Parse(tsss[0]);
                tsss = tsss.Skip(1).ToArray();
                tda = (tsss.Length == 0 || tsss[0] == "") ? null : Array.ConvertAll(tsss, double.Parse);
                tsa = string.Join("", s.Skip(s.IndexOf('|') + 2)).Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                nn.Add(new Tuple<Node, Action<INode>>(new Node(new NodeCtorData(ts, td, tda, td, tsa), out Action<INode> a), a));
            }
            nodes = new List<Node>(nn.Select(x => x.Item1)).ToArray();
            foreach (Action st in nn.Select(x => x.Item2))
            {
                st?.Invoke();
            }
            Array.ForEach(nodes, x => Array.ForEach<string>(x.ChildrenNodes.Select(z => z.NodeIdentifier).ToArray(), y => SearchNode(y).parentNodes.Add(x)));
        }
        /// <summary>
        /// Exports this networks data into a .nnd file to the specified path. (Do not include the file extension in the file path).
        /// </summary>
        /// <param name="fullOutputDataFilePath">The root data path to export to.</param>
        public void ExportNetworkData(string fullOutputDataFilePath)
        {
            StringBuilder sb = new StringBuilder(string.Empty);
            for (int i = 0; i < nodes.Length; i++)
            {
                if ((i + 1) != nodes.Length)
                {
                    //Not the last line.                                        
                    sb.Append((string)nodes[i].NodeIdentifier + "<" + nodes[i].bias.ToString() + ":" + string.Join(":", nodes[i].weights ?? new double[0]) + ">" + "||" + string.Join("|", nodes[i].ChildrenNodes.Select(x => (string)x.NodeIdentifier) ?? new string[0]) + Environment.NewLine);
                }
                else
                {
                    //Last line.
                    sb.Append((string)nodes[i].NodeIdentifier + "<" + nodes[i].bias.ToString() + ":" + string.Join(":", nodes[i].weights ?? new double[0]) + ">" + "||" + string.Join("|", nodes[i].ChildrenNodes.Select(x => (string)x.NodeIdentifier) ?? new string[0]));
                }
            }
            File.WriteAllText(fullOutputDataFilePath + @".nnd", sb.ToString());
        }
        #endregion
    }
}
