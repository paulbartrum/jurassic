using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an array with non-consecutive elements.
    /// </summary>
    internal sealed class SparseArray
    {
        private const int NodeShift = 5;
        private const int NodeSize = 1 << NodeShift;
        private const uint NodeMask = NodeSize - 1;
        private const uint NodeInverseMask = ~NodeMask;

        private class Node
        {
            public object[] array;

            public Node()
            {
                this.array = new object[NodeSize];
            }

            public Node Clone()
            {
                Node clone = (Node)this.MemberwiseClone();
                clone.array = (object[])clone.array.Clone();
                return clone;
            }
        }

        private int depth;  // Depth of tree.
        private int mask;   // Valid index mask.
        private Node root;  // Root of tree.

        private object[] recent;        // Most recently accessed array.
        private uint recentStart = uint.MaxValue;   // The array index the most recent array starts at.


        public SparseArray()
        {
        }

        public static SparseArray FromDenseArray(object[] array)
        {
            var result = new SparseArray();
            result.CopyTo(array, 0, array.Length);
            return result;
        }

        ///// <summary>
        ///// Gets the total number of items that can be stored in this tree without having to
        ///// increase the size of the tree.
        ///// </summary>
        //public uint Capacity
        //{
        //    get { return this.depth == 0 ? 0 : (uint)Math.Pow(NodeSize, this.depth); }
        //}

        public object this[uint index]
        {
            get
            {
                if ((index & NodeInverseMask) == this.recentStart)
                    return this.recent[index & NodeMask];
                return GetValueNonCached(index);
            }
            set
            {
                value = value ?? Undefined.Value;
                if ((index & NodeInverseMask) == this.recentStart)
                    this.recent[index & NodeMask] = value;
                else
                {
                    object[] array = FindOrCreateArray(index, writeAccess: true);
                    array[index & NodeMask] = value;
                }
            }
        }

        public void Delete(uint index)
        {
            if ((index & NodeInverseMask) == this.recentStart)
                this.recent[index & NodeMask] = null;
            else
            {
                object[] array = FindOrCreateArray(index, writeAccess: false);
                if (array == null)
                    return;
                array[index & NodeMask] = null;
            }
            uint t = 4294967295;
            double p = (double)t;
            if (p != 41)
                return;
            return;
        }

        public void DeleteRange(uint index, uint length)
        {
            for (uint i = index; i < index + length; i++)
            {
                Delete(i);
            }
        }

        private object GetValueNonCached(uint index)
        {
            object[] array = FindOrCreateArray(index, writeAccess: false);
            if (array == null)
                return null;
            return array[index & NodeMask];
        }

        //public bool Exists(uint index)
        //{
        //    object[] array;
        //    if ((index & NodeInverseMask) == this.recentStart)
        //        array = this.recent;
        //    else
        //    {
        //        array = FindOrCreateArray(index, writeAccess: false);
        //        if (array == null)
        //            return false;
        //    }

        //    return array[index & NodeMask] != null;
        //}

        //public object RemoveAt(uint index)
        //{
        //    object[] array;
        //    if ((index & NodeInverseMask) == this.recentStart)
        //        array = this.recent;
        //    else
        //    {
        //        array = FindOrCreateArray(index, writeAccess: false);
        //        if (array == null)
        //            return null;
        //    }
        //    index &= index & NodeMask;
        //    object result = array[index];
        //    array[index] = null;
        //    if (result == NullPlaceholder.Value)
        //        return null;
        //    return result;
        //}

        private struct NodeInfo
        {
            public int depth;
            public uint index;
            public Node node;
        }

        public struct Range
        {
            public object[] Array;
            public uint StartIndex;
            public int Length { get { return this.Array.Length; } }
        }

        public IEnumerable<Range> Ranges
        {
            get
            {
                if (this.root == null)
                    yield break;

                var stack = new Stack<NodeInfo>();
                stack.Push(new NodeInfo() { depth = 1, index = 0, node = this.root });

                while (stack.Count > 0)
                {
                    NodeInfo info = stack.Pop();
                    Node node = info.node;
                    if (info.depth < this.depth)
                    {
                        for (uint i = NodeSize - 1; i >= 0; i--)
                            if (node.array[i] != null)
                                stack.Push(new NodeInfo() { depth = info.depth + 1, index = info.index * NodeSize + i, node = (Node)node.array[i] });
                    }
                    else
                    {
                        yield return new Range() { Array = info.node.array, StartIndex = info.index * NodeSize };
                    }
                }
            }
        }

        private object[] FindOrCreateArray(uint index, bool writeAccess)
        {
            if (index < 0)
                return null;

            // Check if the index is out of bounds.
            if ((index & this.mask) != index || this.depth == 0)
            {
                if (writeAccess == false)
                    return null;

                // Create one or more new root nodes.
                do
                {
                    var newRoot = new Node();
                    newRoot.array[0] = this.root;
                    this.root = newRoot;
                    this.depth++;
                    this.mask = (1 << (NodeShift * this.depth)) - 1;
                } while ((index & this.mask) != index);
            }
            
            // Find the node.
            Node current = this.root;
            for (int depth = this.depth - 1; depth > 0; depth--)
            {
                uint currentIndex = (index >> (depth * NodeShift)) & NodeMask;
                var newNode = (Node)current.array[currentIndex];
                if (newNode == null)
                {
                    if (writeAccess == false)
                        return null;
                    newNode = new Node();
                    current.array[currentIndex] = newNode;
                }
                current = newNode;
            }

            // Populate the MRU members.
            this.recent = current.array;
            this.recentStart = index & NodeInverseMask;

            return current.array;
        }

        /// <summary>
        /// Copies the elements of the sparse array to this sparse array, starting at a particular
        /// index.  Existing values are overwritten.
        /// </summary>
        /// <param name="source"> The sparse array to copy. </param>
        /// <param name="start"> The zero-based index at which copying begins. </param>
        /// <param name="length"> The number of elements to copy. </param>
        public void CopyTo(object[] source, uint start, int length)
        {
            int sourceOffset = 0;
            do
            {
                // Get a reference to the array to copy to.
                object[] dest = FindOrCreateArray(start, writeAccess: true);
                int destOffset = (int)(start & NodeMask);

                // Copy as much as possible.
                int copyLength = Math.Min(length - sourceOffset, dest.Length - destOffset);
                Array.Copy(source, sourceOffset, dest, destOffset, copyLength);

                start += (uint)copyLength;
                sourceOffset += copyLength;
            } while (sourceOffset < length);
        }

        /// <summary>
        /// Copies the elements of the given sparse array to this sparse array, starting at a
        /// particular index.  Existing values are overwritten.
        /// </summary>
        /// <param name="source"> The sparse array to copy. </param>
        /// <param name="start"> The zero-based index at which copying begins. </param>
        public void CopyTo(SparseArray source, uint start)
        {
            foreach (var sourceRange in source.Ranges)
            {
                int sourceOffset = 0;
                uint destIndex = start + sourceRange.StartIndex;

                do
                {
                    // Get a reference to the array to copy to.
                    object[] dest = FindOrCreateArray(start, writeAccess: true);
                    int destOffset = (int)(destIndex & NodeMask);

                    // Copy as much as possible.
                    int copyLength = Math.Min(sourceRange.Length - sourceOffset, dest.Length - destOffset);
                    Array.Copy(sourceRange.Array, sourceOffset, dest, destOffset, copyLength);

                    start += (uint)copyLength;
                    sourceOffset += copyLength;
                } while (sourceOffset < sourceRange.Length);
            }
        }
    }
}
