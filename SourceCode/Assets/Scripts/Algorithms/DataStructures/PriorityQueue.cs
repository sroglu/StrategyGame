using System;
using System.Collections;
using System.Collections.Generic;

namespace mehmetsrl.Algorithms.DataStructures
{
    /// <summary>
    /// PriorityQueue implementation. Inspired on a peer programmer code review site.
    /// </summary>


    /// <summary>
    /// Generic queue entry for priority queue
    /// It has key for compare and a value as a queue value.
    /// To compare keys, keys should implement ICompareble interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class QueueEntry<TKey, TValue> : IComparable<QueueEntry<TKey, TValue>> where TKey : IComparable<TKey>
    {
        public QueueEntry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        public TKey Key { get; }
        public TValue Value { get; }

        /// <summary>
        /// Compares the keys.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int CompareTo(QueueEntry<TKey, TValue> other)
        {
            if (EqualityComparer<QueueEntry<TKey, TValue>>.Default.Equals(this, other)) return 0;
            if (other == null) throw new ArgumentNullException();
            return Key.CompareTo(other.Key);
        }
        public override string ToString() => $"Key = {Key}, Value = {Value}";
    }

    /// <summary>
    /// Priority queue that 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class PriorityQueue<TKey, TValue> : IEnumerable<QueueEntry<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        public int Count => items.Count;
        public bool IsEmpty => items.Count == 0;

        private readonly List<QueueEntry<TKey, TValue>> items;
        private TKey minKey;
        private TKey maxKey;
        private int minIndex;
        private int maxIndex;

        public PriorityQueue(int capacity)
        {
            items = new List<QueueEntry<TKey, TValue>>(capacity);
            minKey = default(TKey);
            maxKey = default(TKey);
            minIndex = maxIndex = 0;
        }

        public PriorityQueue() : this(10)
        {
        }

        /// <summary>
        /// Adds new entry for the queue
        /// </summary>
        /// <param name="entry"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(QueueEntry<TKey, TValue> entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            var key = entry.Key;
            if (items.Count == 0) // first item
            {
                minKey = key;
                maxKey = key;
            }
            else if (key.CompareTo(minKey) < 0) // a new min key
            {
                minKey = key;
                minIndex = items.Count; // after the insert there will be one more entry.
            }
            else if (key.CompareTo(maxKey) > 0) // a new max key
            {
                maxKey = key;
                maxIndex = items.Count;
            }
            items.Add(entry);
        }

        /// <summary>
        /// Adds new entry for the queue
        /// </summary>
        /// <param name="entryKey"></param>
        /// <param name="entryValue"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(TKey entryKey, TValue entryValue)
        {
            if (entryKey == null)
            {
                throw new ArgumentNullException(nameof(entryKey));
            }
            if (entryValue == null)
            {
                throw new ArgumentNullException(nameof(entryValue));
            }
            Add(new QueueEntry<TKey, TValue>(entryKey, entryValue));
        }

        //Delete the entry with the min key; O(n).
        public QueueEntry<TKey, TValue> PopMin()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            QueueEntry<TKey, TValue> returnValue = items[minIndex];
            items.RemoveAt(minIndex);
            if (!IsEmpty)
            {
                minIndex = 0;
                for (var i = 1; i < items.Count; i++)
                {
                    if (items[i].Key.CompareTo(items[minIndex].Key) < 0)
                    {
                        minIndex = i;
                    }
                }
                minKey = items[minIndex].Key;
            }
            return returnValue;
        }

        //Delete the entry with the max key; O(n).
        public QueueEntry<TKey, TValue> PopMax()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            QueueEntry<TKey, TValue> returnValue = items[maxIndex];
            items.RemoveAt(maxIndex);
            if (!IsEmpty)
            {
                maxIndex = 0;
                for (var i = 0; i < items.Count; i++)
                {
                    if (items[i].Key.CompareTo(items[maxIndex].Key) > 1)
                    {
                        maxIndex = i;
                    }
                }
                maxKey = items[maxIndex].Key;
            }
            return returnValue;
        }

        //O(1) to get min and max of the queue!
        public QueueEntry<TKey, TValue> Min => IsEmpty ? null : items[minIndex];

        public QueueEntry<TKey, TValue> Max => IsEmpty ? null : items[maxIndex];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<QueueEntry<TKey, TValue>> GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
