
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class SubTreeNode : BTChildNode {

        [System.Serializable]
        public class ExternalPort : NodeDynamicNodePort<BTChildNode> {
            public override IO io => IO.Output;

            public override string name => _name;

            public string _name;
        }


        [Display("Target")]
        [ShowInBody]
        [System.NonSerialized]
        public BehaviourTreeAsset target;

        public List<ExternalPort> ports = new List<ExternalPort>();

        public int binary_search_port(string name) {
            s_comparer.name = name;
            return ports.BinarySearch(null, s_comparer);
        }

        public override void on_serializing(List<Object> referenced_objects) {
            m_target_index = referenced_objects.Count;
            referenced_objects.Add(target);
        }

        public override void on_deserialized(Object[] referenced_objects) {
            target = referenced_objects[m_target_index] as BehaviourTreeAsset;
        }

        private int m_target_index = -1;

        public override BTResult exec(BTExecutorBase executor) {
            if (target == null) {
                return BTResult.failed;
            }
            return BTResult.sub_tree(this);
        }

        private class PortComparer : IComparer<ExternalPort> {
            public string name;
            public int Compare(ExternalPort x, ExternalPort _) {
                return x.name.CompareTo(name);
            }
        }

        private static PortComparer s_comparer = new PortComparer();
    }
}