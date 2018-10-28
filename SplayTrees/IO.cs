using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTrees
{
    /// <summary>
    /// Tree generator, creates the tree and executes commands from input file (insert, find). Also outputs info about the tree to text file.
    /// </summary>
    class TreeGenerator
    {
        private string filename = "";
        private int NodesCount = 0;
        private TreeObserver<int> observer;
        private SplayTreeType type;
        public StreamWriter writer;
        /// <summary>
        /// Initialize tree generator.
        /// </summary>
        /// <param name="input">Name of input file</param>
        /// <param name="output">Name of output file</param>
        /// <param name="typ">Type of the tree to be built/param>
        public TreeGenerator(string input, string output, SplayTreeType typ)
        {
            filename = input;
            writer = new StreamWriter(output);
            type = typ;
        }
        /// <summary>
        /// Creates new tree wrapped in tree observer class, and executes all commands from input file.
        /// </summary>
        /// <returns>Tree created</returns>
        public SplayTree<int> CreateTree()
        {
            observer = new TreeObserver<int>(new SplayTree<int>(type));
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string s = "";
                    while ((s = reader.ReadLine()) != null)
                    {
                        ExecuteCommand(s);
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Cannot read given input file.");
            }
            return observer.Tree;
        }
        /// <summary>
        /// Executes one command from input file.
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        private void ExecuteCommand(string command)
        {
            string[] tokens = command.Split(new char[] { ' ' });
            if (tokens.Length != 2)
            {
                throw new Exception("invalid input");
            }
            switch (tokens[0])
            {
                // new tree
                case "#":
                    if (NodesCount > 0) FinishCurrentTree();
                    NodesCount = int.Parse(tokens[1]);
                    observer.ChangeTree(new SplayTree<int>(type));
                    observer.Tree.NodesCount = NodesCount;
                    break;
                // insert
                case "I":
                    observer.InsertIntoTree(int.Parse(tokens[1]));
                    break;
                // find
                case "F":
                    observer.SearchTree(int.Parse(tokens[1]));
                    break;
            }            
        }
        /// <summary>
        /// Outputs info about tree built to output file - count of nodes and search cost.
        /// </summary>
        public void FinishCurrentTree()
        {
            if (type == SplayTreeType.OPTIMAL)
            {
                double optimalCost = observer.GetOptimalSearchCost();
                writer.WriteLine("{0} {1}", NodesCount, optimalCost);
            }
            else
            {
                double actualCost = observer.GetAverageSearchCost();
                writer.WriteLine("{0} {1}", NodesCount, actualCost);
            }
            //observer.WriteTree();
            //Console.ReadLine();
        }
    }
}
