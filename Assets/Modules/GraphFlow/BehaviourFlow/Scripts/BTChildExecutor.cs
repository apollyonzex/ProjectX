

namespace BehaviourFlow {
    public class BTChildExecutor : BTExecutorBase {
        public override IContext context => m_root.context;
        public override BTExecutorBase parent => m_parent;
        public override BTExecutorBase root => m_root;
        public override BehaviourTreeAsset asset { get => null; protected set { } }

        public BTChildExecutor(BTExecutorBase parent) {
            m_parent = parent;
            m_root = m_parent.root;
#if UNITY_EDITOR
            parent.children.Add(this);
#endif
            init();
        }

        BTExecutorBase m_parent;
        BTExecutorBase m_root;
    }
}