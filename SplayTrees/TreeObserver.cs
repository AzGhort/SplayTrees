using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTrees
{
    /// <summary>
    /// Auxiliary class to count probabilities from sequence of finds.
    /// </summary>
    /// <typeparam name="T">Type of value in the tree.</typeparam>
    class NodeProbabilities<T> where T : IComparable<T>
    {
        public int TotalSearchCount = 0;
        /// <summary>
        /// Holds number of search counts for each value in the tree.
        /// </summary>
        public Dictionary<T, int> SearchCounts = new Dictionary<T, int>();
        /// <summary>
        /// Register new search in the tree.
        /// </summary>
        /// <param name="value">Value being searched.</param>
        public void RegisterSearch(T value)
        {
            TotalSearchCount++;
            SearchCounts[value]++;

        }
        /// <summary>
        /// Register new insert to the tree - e.g. initialize value's search count to zero.
        /// </summary>
        /// <param name="value">Value to be inserted.</param>
        public void RegisterInsert(T value)
        {
            SearchCounts.Add(value, 0);
        }
        /// <summary>
        /// Returns probabilities of searching nodes of the tree.
        /// </summary>
        /// <returns>List of tuples of type Value-Probability.</returns>
        public List<Tuple<T, double>> GetSearchProbs()
        {
            List<Tuple<T, int>> counts = new List<Tuple<T, int>>();
            foreach (KeyValuePair<T, int> pair in SearchCounts)
            {
                Tuple<T, int> tuple = new Tuple<T, int>(pair.Key, pair.Value);
                counts.Add(tuple);
            }
            counts.Add(new Tuple<T, int>(default(T), 0));
            counts.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            counts.Reverse();
            List<Tuple<T,double>> percents = new List<Tuple<T, double>>();
            for (int i = 0; i < counts.Count; i++)
            {
                percents.Add(new Tuple<T, double>(counts[i].Item1, counts[i].Item2 / (1.0*TotalSearchCount)));
            }
            return percents;
        }
    }

    /// <summary>
    /// Observer class wrapping the tree, calls specific method of the tree based on its type.
    /// Also, contains dummy "implementation" of the static optimal tree - in fact, search counts are updated in that case.
    /// </summary>
    /// <typeparam name="T">Type of the values to store in the tree.</typeparam>
    class TreeObserver<T> where T : IComparable<T>
    {
        public SplayTree<T> Tree;
        private NodeProbabilities<T> probs = new NodeProbabilities<T>();
        private double currentMean = 0.0;
        private int currentNumSearchs = 0;
        /// <summary>
        /// Initializes new observer.
        /// </summary>
        /// <param name="tree">Tree to be observed.</param>
        public TreeObserver(SplayTree<T> tree)
        {
            this.Tree = tree;
        }
        /// <summary>
        /// Changes the tree observed, resets all auxiliary stats.
        /// </summary>
        /// <param name="tree">New tree of the observer.</param>
        public void ChangeTree(SplayTree<T> tree)
        {
            Tree = tree;
            probs = new NodeProbabilities<T>();
            currentMean = 0.0;
            currentNumSearchs = 0;
        }
        /// <summary>
        /// Inserts value into observed tree.
        /// </summary>
        /// <param name="value">value to be inserted.</param>
        public void InsertIntoTree(T value)
        {
            if (Tree.type == SplayTreeType.OPTIMAL)
            {
                probs.RegisterInsert(value);
            }
            else
            {
                Tree.Insert(value);
            }
        }
        /// <summary>
        /// Looks for value in the observed tree.
        /// </summary>
        /// <param name="value">Value to be found.</param>
        /// <returns>Value found.</returns>
        public T SearchTree(T value)
        {
            if (Tree.type == SplayTreeType.OPTIMAL)
            {
                probs.RegisterSearch(value);
                return default(T);
            }
            else
            {
                var node = Tree.Find(value).Value;
                UpdateMean();
                return node;
            }
        }
        /// <summary>
        /// Get optimal search cost - e.g. average search cost of static optimal tree.
        /// </summary>
        /// <returns>Average static optimal search cost.</returns>
        public double GetOptimalSearchCost() 
        {
            var probabilities = probs.GetSearchProbs();

            double[,] optimalPathLengths = new double[Tree.NodesCount + 2, Tree.NodesCount + 1];
            double[,] totalWeightOfSubtree = new double[Tree.NodesCount + 2, Tree.NodesCount + 1];
            int[,] optimalRootIndex = new int[Tree.NodesCount + 2, Tree.NodesCount + 2];
                       
            for (int l = 1; l <= Tree.NodesCount; l++)
            {
                for (int i = 1; i <= Tree.NodesCount - l + 1; i++)
                {
                    int j = i + l - 1;
                    optimalPathLengths[i, j] = int.MaxValue;
                    totalWeightOfSubtree[i, j] = totalWeightOfSubtree[i, j - 1] + probabilities[j].Item2;
                    if (i == j)
                    {
                        optimalPathLengths[i, j] = probabilities[j].Item2;
                        optimalRootIndex[i, j] = i;
                        continue;
                    }

                    for (int r = optimalRootIndex[i, j - 1]; r <= optimalRootIndex[i + 1, j]; r++)
                    // CUBIC
                    //for (int r = i; r <= j; r++)
                    {
                        double t = optimalPathLengths[i, r - 1] + optimalPathLengths[r + 1, j] + totalWeightOfSubtree[i, j];
                        if (t < optimalPathLengths[i, j])
                        {
                            optimalPathLengths[i, j] = t;
                            optimalRootIndex[i, j] = r;
                        }
                    }
                }
            }
            return optimalPathLengths[1, Tree.NodesCount];
        }
        /// <summary>
        /// Updates actual average search cost.
        /// </summary>
        private void UpdateMean()
        {
            double mean = ((currentNumSearchs * currentMean) + Tree.LastSearchDepth) / (currentNumSearchs+1);
            currentNumSearchs++;
            currentMean = mean;
        }
        /// <summary>
        /// Get actual average search cost.
        /// </summary>
        /// <returns>Current average seacrh cost.</returns>
        public double GetAverageSearchCost()
        {
            return currentMean;            
        }
        /// <summary>
        /// Auxiliary method, outputs nodes of the observed tree in BFS order.
        /// </summary>
        public void WriteTree() 
        {
            Console.WriteLine("-------------------");
            Console.WriteLine("tree");
            var n = Tree.Root;
            Queue<Node<T>> nodes = new Queue<Node<T>>();
            nodes.Enqueue(n);
            nodes.Enqueue(null);
            while (nodes.Count != 0)
            {
                while (nodes.Count != 0)
                {
                    Node<T> nod = nodes.Dequeue();
                    // end of one level, break
                    if (nod == null) break;
                    Console.Write("{0}   ", nod.Value);

                    // we do not want empty leaves
                    if (nod.Left != null) nodes.Enqueue(nod.Left);
                    if (nod.Right != null) nodes.Enqueue(nod.Right);
                }
                if (nodes.Count != 0) nodes.Enqueue(null);
                Console.WriteLine();
            }
            Console.WriteLine("------------------");
        }
    }
}
