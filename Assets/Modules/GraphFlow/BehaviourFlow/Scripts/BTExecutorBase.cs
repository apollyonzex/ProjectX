

using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow {
    public abstract class BTExecutorBase : System.IDisposable {
        public abstract IContext context { get; }

        public void on_abort(System.Action cb) {
            m_stack[m_stack_top].on_abort = cb;
        }

        public abstract BTExecutorBase parent { get; }
        public abstract BTExecutorBase root { get; }
        public abstract BehaviourTreeAsset asset { get; protected set; }

        public virtual void add_event(string name, BTEvent ev) {
            root.add_event(name, ev);
        }

        public virtual bool try_get_event(string name, out BTEvent ev) {
            return root.try_get_event(name, out ev);
        }

        protected BTExecutorBase() {

        }

        protected void init() {
#if UNITY_EDITOR
            all_instances.Add(this);
            instance_event?.Invoke(this, true);
#endif
        }

        public void dispose() {
            System.GC.SuppressFinalize(this);
            _dispose();
        }

        void System.IDisposable.Dispose() {
            dispose();
        }

        ~BTExecutorBase() {
            _dispose();
        }

        protected enum ChildStatus {
            None = 0,
            Normal,
            Inversed,
            SubTree,
        }

        protected struct Item {
            public IEnumerator<BTResult> enumerator;
            public System.Action on_abort;
            public BTChildNode node;
            public ChildStatus child_status;
#if UNITY_EDITOR
            public BehaviourTreeAsset asset;
#endif
        }

        public bool last_result { get; private set; }

        protected void push(Item item) {
            if (++m_stack_top == m_stack.Length) {
                var stack = new Item[m_stack.Length * 2];
                System.Array.Copy(m_stack, stack, m_stack.Length);
                m_stack = stack;
            }
            m_stack[m_stack_top] = item;
#if UNITY_EDITOR
            if (item.node != null) {
                node_event?.Invoke(item.asset, item.node, NodeEventType.Pending);
            }
            if (item.asset != null) {
                stack_event?.Invoke(item.asset, true);
            }
#endif
        }

        protected virtual void abort() {
            for (int i = m_stack_top; i >= 0; --i) {
                ref var item = ref m_stack[i];
                item.on_abort?.Invoke();
#if UNITY_EDITOR
                if (item.node != null) {
                    node_event?.Invoke(item.asset, item.node, NodeEventType.Abort);
                }
                if (item.asset != null) {
                    stack_event?.Invoke(item.asset, false);
                }
#endif
                item = default;
            }
            m_stack_top = -1;
            last_result = false;
            asset = null;
        }

        protected bool reset_unchecked(BTChildNode node, BehaviourTreeAsset asset, bool exec_once) {
            this.asset = asset;
            push(new Item {
                node = node,
#if UNITY_EDITOR
                asset = asset,
#endif
            });
            if (exec_once) {
                return exec();
            }
            return false;
        }

        public bool reset(BTChildNode node, BehaviourTreeAsset asset, bool exec_once) {
            abort();
            if (node == null) {
                return true;
            }
            return reset_unchecked(node, asset, exec_once);
        }

        public bool exec() {
            while (m_stack_top >= 0) {
            l_again:
                ref var item = ref m_stack[m_stack_top];
                if (item.enumerator == null) {
                    switch (item.child_status) {
                        case ChildStatus.None: {
                            var ret = item.node.exec(this);
                            switch (ret.type) {
                                case BTResultType.Success:
                                    last_result = true;
#if UNITY_EDITOR
                                    node_event?.Invoke(item.asset, item.node, NodeEventType.Success);
#endif
                                    break;

                                case BTResultType.Failed:
                                    last_result = false;
#if UNITY_EDITOR
                                    if (item.node != null) {
                                        node_event?.Invoke(item.asset, item.node, NodeEventType.Failed);
                                    }
#endif
                                    break;

                                case BTResultType.Child: {
                                    if (ret.data is BTChildNode child) {
                                        item.child_status = ChildStatus.Normal;
                                        push(new Item {
                                            node = child,
#if UNITY_EDITOR
                                            asset = item.asset,
#endif
                                        });
                                        goto l_again;
                                    }
                                    last_result = false;
                                    break;
                                }

                                case BTResultType.InversedChild: {
                                    if (ret.data is BTChildNode child) {
                                        item.child_status = ChildStatus.Inversed;
                                        push(new Item {
                                            node = child,
#if UNITY_EDITOR
                                            asset = item.asset,
#endif
                                        });
                                        goto l_again;
                                    }
                                    last_result = false;
                                    break;
                                }

                                case BTResultType.Enumerator:
                                    if (ret.data is IEnumerator<BTResult> enumerator) {
                                        item.enumerator = enumerator;
                                        goto l_again;
                                    }
                                    last_result = false;
                                    break;

                                case BTResultType.Asset: {
                                    if (ret.data is BehaviourTreeAsset asset) {
                                        var graph = asset.graph;
                                        if (graph != null && graph.context_type.IsAssignableFrom(context.context_type)) {
                                            var root = graph.entry.root;
                                            if (root != null) {
                                                item.child_status = ChildStatus.SubTree;
                                                push_sub_tree(null);
                                                item.on_abort = pop_sub_tree;
                                                push(new Item {
                                                    node = root,
#if UNITY_EDITOR
                                                    asset = asset,
#endif
                                                });
                                                goto l_again;
                                            }
                                        }
                                    }
                                    last_result = false;
                                    break;
                                }

                                case BTResultType.SubTree: {
                                    if (ret.data is Nodes.SubTreeNode node) {
                                        var graph = node.target?.graph;
                                        if (graph != null && graph.context_type.IsAssignableFrom(context.context_type)) {
                                            var root = graph.entry.root;
                                            if (root != null) {
                                                item.child_status = ChildStatus.SubTree;
                                                push_sub_tree(node);
                                                item.on_abort = pop_sub_tree;
                                                push(new Item {
                                                    node = root,
#if UNITY_EDITOR
                                                    asset = node.target,
#endif
                                                });
                                                goto l_again;
                                            }
                                        }
                                    }
                                    last_result = false;
                                    break;
                                }


                                case BTResultType.External:
                                    if (ret.data is string name && peek_sub_tree(out var sub_tree, out var sub_tree_asset) && sub_tree != null) {
                                        var idx = sub_tree.binary_search_port(name);
                                        if (idx >= 0) {
                                            var node = sub_tree.ports[idx].value;
                                            if (node != null) {
                                                item.child_status = ChildStatus.Normal;
                                                push(new Item {
                                                    node = node,
#if UNITY_EDITOR
                                                    asset = sub_tree_asset,
#endif
                                                });

                                                goto l_again;
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    last_result = false;
                                    break;
                            }
                            break;
                        }

                        case ChildStatus.Normal:
#if UNITY_EDITOR
                            node_event?.Invoke(item.asset, item.node, last_result ? NodeEventType.Success : NodeEventType.Failed);
#endif
                            break;

                        case ChildStatus.Inversed:
                            last_result = !last_result;
#if UNITY_EDITOR
                            node_event?.Invoke(item.asset, item.node, last_result ? NodeEventType.Success : NodeEventType.Failed);
#endif
                            break;

                        case ChildStatus.SubTree:
                            pop_sub_tree();
#if UNITY_EDITOR
                            if (item.node != null) {
                                node_event?.Invoke(item.asset, item.node, last_result ? NodeEventType.Success : NodeEventType.Failed);
                            }
#endif
                            break;
                    }

                } else {
                l_move_next:
                    if (item.enumerator.MoveNext()) {
                        var ret = item.enumerator.Current;
                        switch (ret.type) {
                            case BTResultType.Success:
#if UNITY_EDITOR
                                if (item.node != null) {
                                    node_event?.Invoke(item.asset, item.node, NodeEventType.Success);
                                }
#endif
                                last_result = true;
                                break;

                            case BTResultType.Pending:
                                return false;

                            case BTResultType.Child: {
                                if (ret.data is BTChildNode child) {
                                    push(new Item {
                                        node = child,
#if UNITY_EDITOR
                                        asset = item.asset,
#endif
                                    });
                                    goto l_again;

                                } 
                                last_result = false;
                                goto l_move_next;
                            }

                            case BTResultType.Enumerator:
                                if (ret.data is IEnumerator<BTResult> enumerator) {
                                    push(new Item {
                                        enumerator = enumerator,
#if UNITY_EDITOR
                                        asset = item.asset,
#endif
                                    });
                                    goto l_again;
                                } 
                                last_result = false;
                                goto l_move_next;

                            case BTResultType.Asset: {
                                if (ret.data is BehaviourTreeAsset asset) {
                                    var graph = asset.graph;
                                    if (graph != null && graph.context_type.IsAssignableFrom(context.context_type)) {
                                        var root = graph.entry.root;
                                        if (root != null) {
                                            push_sub_tree(null);
                                            push(new Item {
                                                child_status = ChildStatus.SubTree,
                                                on_abort = pop_sub_tree,
                                            });
                                            push(new Item {
                                                node = root,
#if UNITY_EDITOR
                                                asset = asset,
#endif
                                            });
                                            goto l_again;
                                        }
                                    }
                                }
                                last_result = false;
                                goto l_move_next;
                            }

                            case BTResultType.SubTree: {
                                if (ret.data is Nodes.SubTreeNode node) {
                                    var graph = node.target?.graph;
                                    if (graph != null && graph.context_type.IsAssignableFrom(context.context_type)) {
                                        var root = graph.entry.root;
                                        if (root != null) {
                                            push_sub_tree(node);
                                            push(new Item {
                                                child_status = ChildStatus.SubTree,
                                                on_abort = pop_sub_tree,
                                            });
                                            push(new Item {
                                                node = root,
#if UNITY_EDITOR
                                                asset = node.target,
#endif
                                            });
                                            goto l_again;
                                        }
                                    }
                                }
                                last_result = false;
                                goto l_move_next;
                            }

                            case BTResultType.External:
                                if (ret.data is string name && peek_sub_tree(out var sub_tree, out var sub_tree_asset) && sub_tree != null) {
                                    var idx = sub_tree.binary_search_port(name);
                                    if (idx >= 0) {
                                        var node = sub_tree.ports[idx].value;
                                        if (node != null) {
                                            push(new Item {
                                                node = node,
#if UNITY_EDITOR

                                                asset = sub_tree_asset,
#endif
                                            });
                                            goto l_again;
                                        }
                                    }
                                }
                                last_result = false;
                                goto l_move_next;

                            default:
                                last_result = false;
#if UNITY_EDITOR
                                if (item.node != null) {
                                    node_event?.Invoke(item.asset, item.node, NodeEventType.Failed);
                                }
#endif
                                break;
                        }
                    } else {
                        last_result = false;
#if UNITY_EDITOR
                        if (item.node != null) {
                            node_event?.Invoke(item.asset, item.node, NodeEventType.Failed);
                        }
#endif
                    }
                }
#if UNITY_EDITOR
                if (item.asset != null) {
                    stack_event?.Invoke(item.asset, false);
                }
#endif
                item = default;
                --m_stack_top;
            }
            asset = null;
            return true;
        }

        protected void _dispose() {
            abort();
#if UNITY_EDITOR
            var parent = this.parent;
            if (parent != null) {
                parent.children.Remove(this);
            }
            all_instances.Remove(this);
            instance_event?.Invoke(this, false);
#endif
        }

        private Item[] m_stack = new Item[8];
        private int m_stack_top = -1;

        private Stack<(Nodes.SubTreeNode, BehaviourTreeAsset)> m_sub_tree_stack = new Stack<(Nodes.SubTreeNode, BehaviourTreeAsset)>();

        public void push_sub_tree(Nodes.SubTreeNode node) {
            m_sub_tree_stack.Push((node, last_asset));
        }

        public void pop_sub_tree() {
            m_sub_tree_stack.Pop();
        }

        public bool peek_sub_tree(out Nodes.SubTreeNode node, out BehaviourTreeAsset asset) {
            if (m_sub_tree_stack.Count > 0) {
                var e = m_sub_tree_stack.Peek();
                node = e.Item1;
                asset = e.Item2;
                return true;
            }
            var parent = this.parent;
            if (parent == null) {
                node = null;
                asset = null;
                return false;
            }
            return parent.peek_sub_tree(out node, out asset);
        }

        public BehaviourTreeAsset last_asset {
            get {
#if UNITY_EDITOR
                return m_stack_top >= 0 ? m_stack[m_stack_top].asset : null;
#else
                return null;
#endif
            }
        }

        public void debug_info(string name, GameObject game_object = null) {
#if UNITY_EDITOR
            this.name = name;
            this.game_object = game_object;
#endif
        }

#if UNITY_EDITOR
        public string name { get; private set; }
        public GameObject game_object { get; private set; }
        public IEnumerable<(BehaviourTreeAsset asset, BTChildNode node)> pending_nodes {
            get {
                for (int i = 0; i <= m_stack_top; ++i) {
                    var item = m_stack[i];
                    if (item.node != null) {
                        yield return (item.asset, item.node);
                    }
                }
            }
        }
        public enum NodeEventType {
            Pending,
            Success,
            Failed,
            Abort,
        }

        public delegate void NodeEvent(BehaviourTreeAsset asset, BTChildNode node, NodeEventType type);
        public event NodeEvent node_event;
        public event System.Action<BehaviourTreeAsset, bool> stack_event;

        public HashSet<BTExecutorBase> children { get; } = new HashSet<BTExecutorBase>();

        public IEnumerable<BTExecutorBase> enumerate_executors() {
            yield return this;
            foreach (var e in children) {
                foreach (var c in e.enumerate_executors()) {
                    yield return c;
                }
            }
        }

        public static event System.Action<BTExecutorBase, bool> instance_event;
        public static HashSet<BTExecutorBase> all_instances { get; } = new HashSet<BTExecutorBase>();
#endif
    }
}