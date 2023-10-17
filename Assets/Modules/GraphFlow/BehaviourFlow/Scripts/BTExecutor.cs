
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow {
    public class BTExecutor : BTExecutorBase {

        public override BehaviourTreeAsset asset { get => m_asset; protected set => m_asset = value; }
        public override IContext context => m_context;
        public override BTExecutorBase parent => null;
        public override BTExecutorBase root => this;

        public override void add_event(string name, BTEvent ev) {
            m_events.Add(name ?? string.Empty, ev);
        }

        public override bool try_get_event(string name, out BTEvent ev) {
            return m_events.TryGetValue(name, out ev);
        }

        public BTExecutor() {
            init();
        }

        public bool reset(IContext context, BehaviourTreeAsset asset, bool exec_once = true) {
            abort();
            var node = asset?.graph.entry.root;
            if (node != null) {
#if UNITY_EDITOR
                if (!asset.graph.context_type.IsAssignableFrom(context?.context_type)) {
                    throw new System.ArgumentException("context is not compatible with graph");
                }
#endif
                m_context = context;
                return reset_unchecked(node, asset, exec_once);
            }
            return true;
        }

        public void discard_events() {
            m_events.Clear();
        }

        BehaviourTreeAsset m_asset;
        IContext m_context;

        Dictionary<string, BTEvent> m_events = new Dictionary<string, BTEvent>();
    }
}