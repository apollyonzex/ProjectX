
using System;
using System.Collections.Generic;

namespace Foundation {

    public interface IBinaryHeapNode<TKey> {
        int index { get; set; }
        TKey key { get; set; }
    }


    public struct BinaryHeap<THeap, TKey, TNode>
            where THeap : IList<TNode>
            where TKey : IComparable<TKey>
            where TNode : class, IBinaryHeapNode<TKey>
        {

        public BinaryHeap(THeap heap) {
            m_heap = heap;
            m_heap.Clear();
        }

        public void push(TNode node) {
            node.index = m_heap.Count;
            m_heap.Add(node);
            sift_up(0, node.index);
        }

        public TNode pop() {
            var last = m_heap.Count - 1;
            if (last < 0) {
                return null;
            }
            var node = m_heap[0];
            node.index = -1;
            if (last == 0) {
                m_heap.Clear();
            } else {
                m_heap[0] = m_heap[last];
                m_heap[0].index = 0;
                m_heap.RemoveAt(last);
                sift_down_to_bottom(0);
            }
            return node;
        }

        public TNode peek() {
            if (m_heap.Count != 0) {
                return m_heap[0];
            }
            return null;
        }

        public void update(int index, TKey key) {
            var ret = key.CompareTo(m_heap[index].key);
            if (ret < 0) {
                m_heap[index].key = key;
                sift_up(0, index);
            } else if (ret > 0) {
                m_heap[index].key = key;
                sift_down(index);
            }
        }

        public bool update_if_less(int index, TKey key) {
            if (key.CompareTo(m_heap[index].key) < 0) {
                m_heap[index].key = key;
                sift_up(0, index);
                return true;
            }
            return false;
        }

        int sift_up(int start, int pos) {
            while (pos > start) {
                var parent = (pos - 1) / 2;
                if (m_heap[pos].key.CompareTo(m_heap[parent].key) >= 0) {
                    break;
                }
                swap(pos, parent);
                pos = parent;
            }
            return pos;
        }

        int sift_down_to_bottom(int pos) {
            var end = m_heap.Count;
            var start = pos;
            var child = 2 * pos + 1;
            while (child < end) {
                var right = child + 1;
                if (right < end && m_heap[right].key.CompareTo(m_heap[child].key) < 0) {
                    child = right;
                }
                swap(pos, child);
                pos = child;
                child = 2 * pos + 1;
            }

            while (pos > start) {
                var parent = (pos - 1) / 2;
                if (m_heap[pos].key.CompareTo(m_heap[parent].key) >= 0) {
                    break;
                }
                swap(pos, parent);
                pos = parent;
            }

            return pos;
        }

        int sift_down(int pos) {
            var end = m_heap.Count;
            var child = 2 * pos + 1;
            while (child < end) {
                var right = child + 1;
                if (right < end && m_heap[right].key.CompareTo(m_heap[child].key) < 0) {
                    child = right;
                }
                if (m_heap[pos].key.CompareTo(m_heap[child].key) <= 0) {
                    break;
                }
                swap(pos, child);
                child = 2 * pos + 1;
            }
            return pos;
        }

        void swap(int i, int j) {
            var a = m_heap[i];
            var b = m_heap[j];
            
            m_heap[i] = b;
            b.index = i;

            m_heap[j] = a;
            a.index = j;
        }

        THeap m_heap;
    }
}
