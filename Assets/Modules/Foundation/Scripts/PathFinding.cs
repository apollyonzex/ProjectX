
using System;
using System.Collections;
using System.Collections.Generic;


namespace Foundation {

    public enum PathFindingNodeStatus {
        None,
        Open,
        Close,
    }

    public interface IPathFindingArc<TNode, TCost> {
        TCost cost { get; }
        TNode target { get; }
    }

    public interface IPathFindingNode<TNode, TCost, TArc> : IBinaryHeapNode<TCost>
            where TCost : IComparable<TCost>
            where TNode : IPathFindingNode<TNode, TCost, TArc>
            where TArc : IPathFindingArc<TNode, TCost>
        {
        PathFindingNodeStatus status { get; set; }
        TNode from { get; set; }
        TArc arc { get; set; }
        TCost h { get; set; }
        TCost g { get; set; }
        int step { get; set; }
    }

    public interface IPathFindingNodeAdjacenciesBuilder<TNode, TAdjacencies> {
        TAdjacencies get_adjacencies(TNode node);
    }

    public interface IPathFindingCostSummator<TCost> {
        TCost zero { get; }
        TCost summate(TCost a, TCost b);
    }

    public interface IPathFindingCostEvaluator<TNode, TCost> {
        TCost evaluate(TNode node);
        bool is_closer(TNode bh_top, TNode last);
    }

    public struct PathFindingReversedPath<TNode, TCost, TArc> : IEnumerator<TNode>
            where TCost : IComparable<TCost>
            where TNode : class, IPathFindingNode<TNode, TCost, TArc>
            where TArc : IPathFindingArc<TNode, TCost>
        {            
            public TNode target { get; }
            public PathFindingReversedPath(TNode node) {
                target = node;
                Current = null;
            }

            public int copy_arcs_to(TArc[] arcs) {
                if (target != null) {
                    var node = target;
                    var count = node.step;
                    var capacity = arcs.Length;
                    var ignore_count = count - capacity;
                    while (ignore_count > 0) {
                        node = node.from;
                        --ignore_count;
                    }
                    count = node.step;
                    for (int i = 0; i < count; ++i) {
                        arcs[node.step - 1] = node.arc;
                        node = node.from;
                    }
                    return count;
                }
                return 0;
            }

            public TNode Current { get; private set; }

            object IEnumerator.Current => Current;

            void IDisposable.Dispose() {
                
            }

            public bool MoveNext() {
                if (Current == null) {
                    if (target == null) {
                        return false;
                    }
                    Current = target;
                } else {
                    if (Current.from == null) {
                        return false;
                    }
                    Current = Current.from;
                }
                return true;
            }

            public void Reset() {
                Current = null;
            }
        }

    public struct PathFinding<THeap, TNode, TCost, TArc, TAdjacenciesBuilder, TAdjacencies, TEvaluator, TSummator>
            where THeap : IList<TNode>
            where TNode : class, IPathFindingNode<TNode, TCost, TArc>
            where TCost : IComparable<TCost>
            where TArc : IPathFindingArc<TNode, TCost>
            where TAdjacenciesBuilder : IPathFindingNodeAdjacenciesBuilder<TNode, TAdjacencies>
            where TAdjacencies: IEnumerator<TArc>
            where TEvaluator : IPathFindingCostEvaluator<TNode, TCost>
            where TSummator : IPathFindingCostSummator<TCost>
        {

        public PathFinding(THeap heap, TNode start, TEvaluator evaluator, TAdjacenciesBuilder adjacencies_builder, TSummator summator) {
            m_bh = new BinaryHeap<THeap, TCost, TNode>(heap);
            m_evaluator = evaluator;
            m_summator = summator;
            m_adjacencies_builder = adjacencies_builder;

            start.h = m_summator.zero;
            start.g = m_evaluator.evaluate(start);
            start.from = null;
            start.key = start.g;
            start.status = PathFindingNodeStatus.Open;
            start.step = 0;
            m_bh.push(start);

            closest = start;
        }

        public static PathFinding<THeap, TNode, TCost, TArc, TAdjacenciesBuilder, TAdjacencies, TEvaluator, TSummator> multi_starts<T>(THeap heap, T starts, TEvaluator evaluator, TAdjacenciesBuilder adjacencies_builder, TSummator summator)
            where T : IEnumerator<TNode>    
        {
            var self = new PathFinding<THeap, TNode, TCost, TArc, TAdjacenciesBuilder, TAdjacencies, TEvaluator, TSummator> {
                m_bh = new BinaryHeap<THeap, TCost, TNode>(heap),
                m_evaluator = evaluator,
                m_summator = summator,
                m_adjacencies_builder = adjacencies_builder,
            };

            while (starts.MoveNext()) {
                var start = starts.Current;
                start.h = self.m_summator.zero;
                start.g = self.m_evaluator.evaluate(start);
                start.from = null;
                start.key = start.g;
                start.status = PathFindingNodeStatus.Open;
                start.step = 0;
                self.m_bh.push(start);
            }

            self.closest = self.m_bh.peek();

            return self;
        }

        public TNode closest { get; private set; }

        public PathFindingReversedPath<TNode, TCost, TArc> reversed_path => new PathFindingReversedPath<TNode, TCost, TArc>(closest);

        public bool advance() {
            var best = m_bh.pop();
            if (best == null) {
                return false;
            }
            best.status = PathFindingNodeStatus.Close;
            var iter = m_adjacencies_builder.get_adjacencies(best);
            while (iter.MoveNext()) {
                var arc = iter.Current;
                var n = arc.target;
                switch (n.status) {
                    case PathFindingNodeStatus.None:
                        n.from = best;
                        n.arc = arc;
                        n.h = m_summator.summate(best.h, arc.cost);
                        n.g = m_evaluator.evaluate(n);
                        n.key = m_summator.summate(n.h, n.g);
                        n.status = PathFindingNodeStatus.Open;
                        n.step = best.step + 1;
                        m_bh.push(n);
                        break;
                    case PathFindingNodeStatus.Close: continue;
                    case PathFindingNodeStatus.Open: {
                        var f = m_summator.summate(best.h, arc.cost);
                        if (f.CompareTo(n.h) < 0) {
                            n.from = best;
                            n.arc = arc;
                            n.h = f;
                            n.step = best.step + 1;
                            m_bh.update_if_less(n.index, m_summator.summate(f, n.g));
                        }
                        break;
                    }
              
                }
            }
            var node = m_bh.peek();
            if (node != null && m_evaluator.is_closer(node, closest)) {
                closest = node;
            }
            return true;
        }

        TAdjacenciesBuilder m_adjacencies_builder;
        TEvaluator m_evaluator;
        TSummator m_summator;
        BinaryHeap<THeap, TCost, TNode> m_bh;
    }

    public struct PathFindingFloatSummator : IPathFindingCostSummator<float> {
        public float zero => 0f;
        public float summate(float a, float b) {
            return a + b;
        }
    }

    public struct PathFindingIntSummator : IPathFindingCostSummator<int> {
        public int zero => 0;
        public int summate(int a, int b) {
            return a + b;
        }
    }
}