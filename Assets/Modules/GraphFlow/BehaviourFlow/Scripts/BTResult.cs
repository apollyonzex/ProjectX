
using System.Collections.Generic;

namespace BehaviourFlow {
    
    public enum BTResultType {
        Success,
        Failed,
        Pending,
        Child,
        InversedChild,
        Enumerator,
        Asset,
        SubTree,
        External,
    }

    public struct BTResult {
        public BTResultType type;
        public object data;

        public static BTResult success => new BTResult { type = BTResultType.Success };
        public static BTResult failed => new BTResult { type = BTResultType.Failed };
        public static BTResult pending => new BTResult { type = BTResultType.Pending };
        public static BTResult child(BTChildNode child) => new BTResult { type = BTResultType.Child, data = child };
        public static BTResult inversed_child(BTChildNode child) => new BTResult { type = BTResultType.InversedChild, data = child };
        public static BTResult enumerator(IEnumerator<BTResult> enumerator) => new BTResult { type = BTResultType.Enumerator, data = enumerator };
        public static BTResult asset(BehaviourTreeAsset asset) => new BTResult { type = BTResultType.Asset, data = asset };
        public static BTResult sub_tree(Nodes.SubTreeNode sub_tree) => new BTResult { type = BTResultType.SubTree, data = sub_tree };
        public static BTResult external(string name) => new BTResult { type = BTResultType.External, data = name };
    }

}