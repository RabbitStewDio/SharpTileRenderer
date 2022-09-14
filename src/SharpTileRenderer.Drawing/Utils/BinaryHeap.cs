using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpTileRenderer.Drawing.Utils
{
    //Binary Heap

    /// Binary heap implementation. Binary heaps are really fast for ordering
    /// nodes in a way that makes it possible to get the node with the lowest F
    /// score. Also known as a priority queue. * \see
    /// http://en.wikipedia.org/wiki/Binary_heap * \see
    /// http://weblogs.asp.net/cumpsd/archive/2005/02/13/371719.aspx *  * This
    /// class ignores the first entry in the array to simplify the mathematics
    /// used. *  * The children of any given item are always stored at the item’s
    /// location * 2 and the item’s location * 2 + 1. For example, in the image
    /// given above,  * the item with value 20 is stored at index 3 and its two
    /// children can be found at index 6 (3 * 2) and index 7 (3 * 2 + 1). */
    public sealed class BinaryHeap<T>
    {
        T[] data;
        readonly IComparer<T> comparer;
        int numberOfItems;

        public BinaryHeap(int numberOfElements, 
                          IComparer<T>? comparer = null)
        {
            if (numberOfElements <= 0)
            {
                throw new ArgumentException($"NumberOfElements must be larger than 0, but found {numberOfElements}", nameof(numberOfElements));
            }

            this.data = new T[1 + numberOfElements];
            this.numberOfItems = 1;
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        public int Size
        {
            get { return numberOfItems - 1; }
        }

        void HandleSizeLimitReached()
        {
            Array.Resize(ref data, Math.Max(numberOfItems + 100, numberOfItems * 3 / 2));
        }

        /** Adds a node to the heap */
        public void Add(in T node)
        {
            if (numberOfItems == data.Length)
            {
                HandleSizeLimitReached();
            }

            data[numberOfItems] = node;

            var bubbleIndex = numberOfItems;

            while (bubbleIndex != 1)
            {
                var parentIndex = bubbleIndex / 2;

                if (comparer.Compare(node, data[parentIndex]) <= 0)
                {
                    Swap(parentIndex, bubbleIndex);
                    bubbleIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            numberOfItems++;
            Revalidate();
        }

        public void Clear()
        {
            this.numberOfItems = 1;
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = default!;
            }
        }

        /** Returns the node with the lowest F score from the heap */
        public T Remove()
        {
            return Remove(0);
        }

        public T Remove(int index)
        {
            if (numberOfItems <= 1)
            {
                throw new InvalidOperationException("Empty heap");
            }

            if (index < 0)
            {
                throw new IndexOutOfRangeException($"Index {index} is invalid.");
            }

            if (index >= Size)
            {
                throw new IndexOutOfRangeException($"Index {index} is invalid.");
            }

            return RemoveRaw(index + 1);
        }

        public T RemoveRaw(int index)
        {
            if (index < 1 || index > Size)
            {
                throw new ArgumentOutOfRangeException($"{index} must be greater than zero and less than {Size}");
            }

            numberOfItems--;

            if (index == numberOfItems)
            {
                var returnItemX = data[index];
                data[index] = default!;
                return returnItemX;
            }

            var returnItem = data[index];
            data[index] = data[numberOfItems];
            data[numberOfItems] = default!;

            var swapItem = index;

            // removing a node: Take the first node out. Now move the last node (largest node) to the top and trickle it downwards.

            while(true)
            {
                var parent = swapItem;
                var p2 = parent * 2;
                if ((p2 + 1) < numberOfItems)
                {
                    // Both children exist
                    if (comparer.Compare(data[parent], data[p2]) >= 0)
                    {
                        swapItem = p2; //2 * parent;
                    }

                    if (comparer.Compare(data[swapItem], data[p2 + 1]) >= 0)
                    {
                        swapItem = p2 + 1;
                    }
                }
                else if ((p2) < numberOfItems)
                {
                    // Only one child exists
                    if (comparer.Compare(data[parent], data[p2]) >= 0)
                    {
                        swapItem = p2;
                    }
                }

                // One if the parent's children are smaller or equal, swap them
                if (parent == swapItem)
                {
                    break;
                }
                
                Swap(parent, swapItem);
            }

            Revalidate();
            return returnItem;
        }

        public T this[int idx]
        {
            get
            {
                if (idx < 0 || idx >= numberOfItems)
                {
                    throw new IndexOutOfRangeException();
                }
                
                return data[idx + 1];
            }
        }
        
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        void Revalidate()
        {
            return;
#pragma warning disable 162
            for (var i = 2; i < numberOfItems; i += 1)
            {
                var item = data[i];
                var parent = data[i / 2];
                if (comparer.Compare(parent, item) > 1)
                {
                    throw new ArgumentException("Unable to validate entry: " + i + " := " + item);
                } 
            }
#pragma warning restore 162
        }
        
        void Swap(int parentIndex, int bubbleIndex)
        {
            (data[parentIndex], data[bubbleIndex]) = (data[bubbleIndex], data[parentIndex]);
        }

        /** Returns a nicely formatted string describing the tree structure. '!!!' marks after a value means that the tree is not correct at that node (i.e it should be swapped with it's parent) */
        public override string ToString()
        {
            var text = new StringBuilder();

            text.Append("\n=== Writing Binary Heap ===\n");
            text.Append("Number of items: ").Append(numberOfItems).Append(" => Count: ").Append(numberOfItems - 1)
                .Append("\n");
            text.Append("Capacity: ").Append(data.Length);
            text.Append("\n");
            if (numberOfItems > 1)
            {
                WriteBranch(1, 1, text);
            }

            text.Append("\n\n");
            return text.ToString();
        }

        /** Writes a branch of the tree to a StringBuilder. Used by #ToString */
        void WriteBranch(int index, int depth, StringBuilder text)
        {
            text.Append("\n");
            for (var i = 0; i < depth; i++)
            {
                text.Append("   ");
            }

            text.Append(index);
            text.Append(":");
            text.Append(data[index]);

            if (index > 1)
            {
                var parentIndex = index / 2;

                if (comparer.Compare(data[index], data[parentIndex]) < 0)
                {
                    text.Append("  !!!");
                }
            }

            var p2 = index * 2;
            if ((p2 + 1) < numberOfItems)
            {
                // Both children exist
                WriteBranch(p2, depth + 1, text);
                WriteBranch(p2 + 1, depth + 1, text);
            }
            else if (p2 < numberOfItems)
            {
                // Only one child exists
                WriteBranch(p2, depth + 1, text);
            }
        }

        public int Capacity => data.Length - 1;
        
        public void Resize(int numberOfElements)
        {
            if (numberOfElements <= 1)
            {
                throw new ArgumentException($"NumberOfElements must be larger than 0, but found {numberOfElements}", nameof(numberOfElements));
            }

            if (data.Length != 1 + numberOfElements)
            {
                Array.Resize(ref data, 1 + numberOfElements);
            }
        }
    }
    
}