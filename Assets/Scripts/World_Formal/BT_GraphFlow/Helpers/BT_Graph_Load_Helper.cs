using World_Formal.BT_GraphFlow.Nodes;
using GraphNode;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common_Formal;
using System;
using World_Formal.BT_GraphFlow.Nodes.DS_Nodes;

namespace World_Formal.BT_GraphFlow.Helpers
{
    [Serializable]
    public class BT_Graph_Load_Helper : Singleton<BT_Graph_Load_Helper>
    {
        [Serializable]
        public class Node_Info
        {
            public BT_Node self;
            public Dictionary<NodePort, (NodePort, BT_Node)> parents = new();
            public Dictionary<NodePort, (NodePort, BT_Node)> childs = new();
        }

        [Serializable]
        public class Step
        {
            public BT_Node self;
            public MethodInfo method_info;
        }


        Dictionary<BT_Node, Node_Info> node_info_dic = new();
        StartNode start = new();
        BT_Node current;
        BT_Node next;

        //================================================================================================

        public void load_to_context(BT_Context ctx, Graph graph)
        {
            get_do_list(graph, out ctx.do_list);
            get_all_cpns(ctx, graph, out ctx.cpns_dic);
            ctx.static_prms = new object[] { ctx };
        }


        void get_do_list(Graph graph, out List<Step> do_list)
        {
            node_info_dic.Clear();

            //解析连接，得到nodes
            foreach (var cnn in graph.connections)
            {
                if (cnn.input_node is not BT_Node child) continue;
                if (cnn.output_node is not BT_Node self) continue;

                if (self is StartNode _start) start = _start;

                if (node_info_dic.ContainsKey(self))
                {
                    ref var childs = ref node_info_dic[self].childs;
                    childs.Add(cnn.output_port, (cnn.input_port, child));
                    childs = childs.OrderBy(e => child.get_method_seq(self, e.Key.name)).ToDictionary(e => e.Key, e => e.Value);
                }
                else
                {
                    Node_Info info = new() { self = self };
                    info.childs.Add(cnn.output_port, (cnn.input_port, child));
                    node_info_dic.Add(self, info);
                }

                if (node_info_dic.ContainsKey(child))
                {
                    ref var parents = ref node_info_dic[child].parents;
                    node_info_dic[child].parents.Add(cnn.input_port, (cnn.output_port, self));
                    parents = parents.OrderBy(e => self.get_method_seq(child, e.Key.name)).ToDictionary(e => e.Key, e => e.Value);
                }
                else
                {
                    Node_Info info = new() { self = child };
                    info.parents.Add(cnn.input_port, (cnn.output_port, self));
                    node_info_dic.Add(child, info);
                }
            }

            //生成路径
            current = start;
            next = null;
            bool is_end = false;

            do_list = new()
            {
                new()
                {
                    self = start,
                    method_info = get_method(start)
                }
            };

            while (!is_end)
            {
                if (next != null)
                    current = next;

                if (!node_info_dic[current].childs.Any())
                {
                    to_former_node(ref do_list, current, out next);
                }
                else
                {
                    ref var childs = ref node_info_dic[current].childs;
                    var first = childs.First();
                    next = first.Value.Item2;
                    childs.Remove(first.Key);
                }

                do_list.Add(new()
                {
                    self = next,
                    method_info = get_method(next)
                });

                is_end = next is EndNode;
            }
        }


        void to_former_node(ref List<Step> do_list, BT_Node current, out BT_Node former)
        {
            ref var parents = ref node_info_dic[current].parents;
            var first = parents.First();

            former = first.Value.Item2;
            parents.Remove(first.Key);

            current = former;
            do_list.Add(new()
            {
                self = next,
                method_info = get_method_back(next)
            });

            if (!node_info_dic[current].childs.Any())
                to_former_node(ref do_list, current, out former);
        }


        MethodInfo get_method(BT_Node current)
        {
            var type = current.GetType();
            var childs = node_info_dic[current].childs;

            if (childs.Any())
            {
                var output = childs.First().Key;
                var methods = type.GetMethods();

                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<BT_GraphFlow.DisplayAttribute>();
                    if (attr == null) continue;

                    if (attr.name == output.name)
                    {
                        return method;
                    }
                }
            }

            return type.GetMethod("do_self");
        }


        MethodInfo get_method_back(BT_Node current)
        {
            var type = current.GetType();
            return type.GetMethod("do_back");
        }


        void get_all_cpns(BT_Context ctx, Graph graph, out Dictionary<string, BT_CPN> cpns_dic)
        {
            cpns_dic = new();
            foreach (var e in graph.nodes)
            {
                if (e is not BT_DSNode node) continue;

                var name = node.module_name;
                if (!name.Any()) continue;

                var cpn = node.init_cpn<BT_CPN>();
                cpn.init(node);
                cpns_dic.Add(name, cpn);
                cpn.init(ctx);
            }
        }
    }
}

