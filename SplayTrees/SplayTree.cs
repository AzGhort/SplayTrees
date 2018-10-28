using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTrees
{
    /// <summary>
    /// Type of the splay tree - STANDARD, NAIVE and OPTIMAL.
    /// </summary>
    enum SplayTreeType
    { NAIVE, STANDARD, OPTIMAL }

    /// <summary>
    /// SplayTree class.
    /// </summary>
    /// <typeparam name="T">Type of value to store in the tree.</typeparam>
    class SplayTree<T> where T : IComparable<T>
    {
        public Node<T> Root { get; private set; }
        private Node<T> current = null;
        public SplayTreeType type;
        // for the purpose of homework, counts how many nodes are there in the tree
        public int NodesCount = 0;
        public int LastSearchDepth = 1;
        /// <summary>
        /// Initializes new tree.
        /// </summary>
        /// <param name="type">Type of the tree.</param>
        public SplayTree(SplayTreeType type)
        {
            this.type = type;
        }
        /// <summary>
        /// Inserts a new node to the tree.
        /// </summary>
        /// <param name="value">Value of the new node.</param>
        public void Insert(T value)
        {
            if (Root == null)
            {
                Node<T> n = null;
                if (type == SplayTreeType.NAIVE) n = new Node<T>(value);
                else if (type == SplayTreeType.STANDARD) n = new StandardNode<T>(value);
                Root = n;
                current = Root;
                return;
            }
            Node<T> found = LookFor(value);
            // should always ?!?!
            if (found == null)
            {
                Node<T> n = null;
                if (type == SplayTreeType.NAIVE) n = new Node<T>(value, current, null, null);
                else if (type == SplayTreeType.STANDARD) n = new StandardNode<T>(value, current, null, null);
                if (value.CompareTo(current.Value) > 0)
                {
                    current.Right = n;
                }
                else
                {
                    current.Left = n;
                }
                current = n;
                // splay the tree
                Splay();
            }
        }
        /// <summary>
        /// Finds the node with given value.
        /// </summary>
        /// <param name="val">Value to find</param>
        /// <returns></returns>
        public Node<T> Find(T val)
        {
            LastSearchDepth = 1;
            //actually find the node
            Node<T> ret = LookFor(val);
            // splay
            if (ret != null) Splay();
            return ret;
        }
        /// <summary>
        /// Looks for node with the given value, returns null if it does not exist in the tree.
        /// Also sets the this.current node to parent of potential node (because of insertion).
        /// </summary>
        /// <param name="val">Value to loof for in the tree.</param>
        /// <returns>Node found or null.</returns>
        private Node<T> LookFor(T val)
        {
            Node<T> current = Root;
            while (val.CompareTo(current.Value) != 0)
            {
                if (val.CompareTo(current.Value) > 0)
                {
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }
                if (current == null) break;
                this.current = current;
                LastSearchDepth++;
            }
            return current;
        }
        /// <summary>
        /// Splays the current node up to the root.
        /// </summary>
        private void Splay()
        {
            while (current.Father != null)
            {
                current.RotateUp();
            }
            Root = current;            
        }         
    }
}
