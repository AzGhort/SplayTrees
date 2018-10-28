using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplayTrees
{
    /// <summary>
    /// Basic "naive" node class for splay tree, implements basic methods (simple rotations, ...).
    /// </summary>
    /// <typeparam name="T">Type of value to store in the tree.</typeparam>
    class Node<T> where T : IComparable<T>
    {
        public T Value { get; protected set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }
        public Node<T> Father { get; set; }
        /// <summary>
        /// Creates new node.
        /// </summary>
        /// <param name="value">Value to store.</param>
        /// <param name="father">Pointer to father.</param>
        /// <param name="left">Pointer to left son.</param>
        /// <param name="right">Pointer to right son.</param>
        public Node(T value, Node<T> father = null, Node<T> left = null, Node<T> right = null)
        {
            Value = value;
            Left = left;
            Right = right;
            Father = father;
        }
        public Node()
        {

        }
        /// <summary>
        /// Basic naive implementation of rotate up - just calls the zig rotate.
        /// </summary>
        public virtual void RotateUp()
        {
            //naive, just call the zig rotate
            ZigRotate();
        }
        /// <summary>
        /// Performs single rotation (if current node is not the root).
        /// </summary>
        public void ZigRotate()
        {
            if (Father == null) return;
            if (Father.Left == this)
            {
                RightRotate();
            }
            else
            {
                LeftRotate();
            }
        }
        /// <summary>
        /// Performs right rotation
        /// </summary>
        protected void RightRotate()
        {
            Node<T> father = Father;
            Node<T> grandfather = Father.Father;
            Node<T> B = Right;

            bool left = true;
            // cut the changing part off the tree
            if (grandfather != null)
            {
                if (father == grandfather.Left)
                {
                    grandfather.Left = null;
                }
                else if (father == grandfather.Right)
                {
                    grandfather.Right = null;
                    left = false;
                }
            }

            father.Left = B;
            if (B != null) B.Father = father;
            Father = grandfather;
            Right = father;
            father.Father = this;

            if (grandfather != null)
            {
                //stick to the rest of tree
                if (left) grandfather.Left = this;
                else grandfather.Right = this;
            }
            // the root !!
            // else Root = this;
        }
        /// <summary>
        /// Performs left rotation
        /// </summary>
        protected void LeftRotate()
        {
            Node<T> father = Father;
            Node<T> grandfather = Father.Father;
            Node<T> B = Left;

            bool left = true;
            // cut the changing part off the tree
            if (grandfather != null)
            {
                if (father == grandfather.Left)
                {
                    grandfather.Left = null;
                }
                else if (father == grandfather.Right)
                {
                    grandfather.Right = null;
                    left = false;
                }
            }

            father.Right = B;
            if (B != null) B.Father = father;
            Father = grandfather;
            Left = father;
            father.Father = this;

            if (grandfather != null)
            {
                //stick to the rest of tree
                if (left) grandfather.Left = this;
                else grandfather.Right = this;
            }
            // the root
            // else Root = node;
        }
    }  
   
    /// <summary>
    /// Extended "standard" variant of node for splay tree, handles double rotations
    /// </summary>
    /// <typeparam name="T">Type of value to store in the tree</typeparam>
    class StandardNode<T> : Node<T> where T : IComparable<T>
    {
        /// <summary>
        /// Creates new node.
        /// </summary>
        /// <param name="value">Value to store.</param>
        /// <param name="father">Pointer to father.</param>
        /// <param name="left">Pointer to left son.</param>
        /// <param name="right">Pointer to right son.</param>
        public StandardNode(T value, Node<T> father = null, Node<T> left = null, Node<T> right = null)
        {
            Value = value;
            Left = left;
            Right = right;
            Father = father;
        }
        /// <summary>
        /// Rotates the node up - single rotation if father is root, double rotation otherwise.
        /// </summary>
        public override void RotateUp()
        {
            if (Father == null) return;
            Node<T> grandfather = Father.Father;
            // father is root -> just simple rotation
            if (grandfather == null)
            {
                ZigRotate();
            }
            else // two (four) cases of double rotation
            {
                // zig zig
                if ((grandfather.Left == Father && Father.Left == this) || (grandfather.Right == Father && Father.Right == this))
                {
                    ZigZigRotate();
                }
                // zig zag
                else if ((grandfather.Left == Father && Father.Right == this) || (grandfather.Right == Father && Father.Left == this))
                {
                    ZigZagRotate();
                }           
            }
        }      
        /// <summary>
        /// Performs the zig-zig rotation (right-right or left-left).
        /// </summary>
        private void ZigZigRotate()
        {
            Father.ZigRotate();
            ZigRotate();
        }     
        /// <summary>
        /// Performs the zig-zag rotation (right-left or left-right).
        /// </summary>
        private void ZigZagRotate()
        {
            ZigRotate();
            ZigRotate();
        }        
    }
}
