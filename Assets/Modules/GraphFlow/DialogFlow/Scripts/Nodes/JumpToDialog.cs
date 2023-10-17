
using GraphNode;
using System.Collections.Generic;
using UnityEngine;

namespace DialogFlow.Nodes {

    [System.Serializable]
    [Graph(typeof(DialogGraph))]
    public class JumpToDialog : DialogNode {

        [System.Serializable]
        public class ExternalPort : NodeDelegatePort<DialogAction> {
            public override IO io => IO.Output;

            public override string name => _name;

            public override bool can_mulit_connect => false;

            public string _name;
        }

        [System.Serializable]
        public class OptionStateExternalPort : NodeDelegatePort<OptionStateFunc> {
            public override IO io => IO.Input;
            public override string name => _name;
            public override bool can_mulit_connect => false;

            public string _name;
        }

        [System.Serializable]
        public class OptionsExternalPort : NodeDynamicNodePort<PageOptionsBase> {
            public override IO io => IO.Output;

            public override string name => _name;

            public string _name;
        }


        [Display("Target")]
        [ShowInBody]
        [System.NonSerialized]
        public DialogGraphAsset target;

        public List<ExternalPort> ports = new List<ExternalPort>();
        public List<OptionStateExternalPort> option_states = new List<OptionStateExternalPort>();
        public List<OptionsExternalPort> options = new List<OptionsExternalPort>();

        public int binary_search_port(string name) {
            s_comparer.name = name;
            return ports.BinarySearch(null, s_comparer);
        }

        public int binary_search_option_state(string name) {
            s_comparer.name = name;
            return option_states.BinarySearch(null, s_comparer);
        }

        public int binary_search_options(string name) {
            s_comparer.name = name;
            return options.BinarySearch(null, s_comparer);
        }


        [Input(can_multi_connect = true)]
        [Display("")]
        public void invoke(IContext context) {
            if (target != null) {
                context.push_externals(this);
                target.graph.entry.invoke(context);
            } else {
                context.end();
            }
        }

        public override void on_serializing(List<Object> referenced_objects) {
            m_target_index = referenced_objects.Count;
            referenced_objects.Add(target);
        }

        public override void on_deserialized(Object[] referenced_objects) {
            target = referenced_objects[m_target_index] as DialogGraphAsset;
        }

        private int m_target_index = -1;

        private class PortComparer : IComparer<NodeDynamicPort> {
            public string name;
            public int Compare(NodeDynamicPort x, NodeDynamicPort _) {
                return x.name.CompareTo(name);
            }
        }

        private static PortComparer s_comparer = new PortComparer();
    }
}