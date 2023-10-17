
using System.Collections.Generic;
using CalcExpr;

namespace BehaviourFlow.Exports {
    public class Exporter {
        public Exporter(BehaviourTree graph) {
            graph.entry?.root?.export(this, out _);
        }

        public int get_constant_index(uint key) {
            if (!m_constants.TryGetValue(key, out var index)) {
                index = m_constants.Count;
                m_constants.Add(key, index);
            }
            return index;
        }

        public int get_string_index(string key) {
            if (!m_strings.TryGetValue(key, out var index)) {
                index = m_strings.Count;
                m_strings.Add(key, index);
            }
            return index;
        }

        public int get_shared_int_index(string key) {
            if (!m_shared_ints.TryGetValue(key, out var value)) {
                value.name_index = get_string_index(key);
                value.index = m_shared_ints.Count;
                m_shared_ints.Add(key, value);
            }
            return value.index;
        }

        public int get_shared_float_index(string key) {
            if (!m_shared_floats.TryGetValue(key, out var value)) {
                value.name_index = get_string_index(key);
                value.index = m_shared_floats.Count;
                m_shared_floats.Add(key, value);
            }
            return value.index;
        }

        public int add_expression(uint[] code, IExpressionExternal[] externals, ExpressionFunction[] functions) {
            var index = m_expressions.Count;
            var byte_code = new Foundation.Packets.List<Foundation.Packets.u32>(new Foundation.Packets.u32[code.Length]);
            for (int i = 0; i < code.Length; ++i) {
                byte_code.items[i] = code[i];
            }
            var _externals = new Foundation.Packets.List<AutoCode.Packets.BehaviourFlowExports.ExpressionExternal>();
            if (externals != null) {
                _externals.items = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal[externals.Length];
                for (int i = 0; i < externals.Length; ++i) {
                    var external = externals[i];
                    AutoCode.Packets.BehaviourFlowExports.ExpressionExternal e;
                    if (external is Constant constant) {
                        e = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal {
                            et = AutoCode.Packets.BehaviourFlowExports.ExternalType.Constant,
                            index = (ulong)get_constant_index(constant.value),
                        };
                    } else if (external is ContextSharedInt shared_int) {
                        e = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal {
                            et = AutoCode.Packets.BehaviourFlowExports.ExternalType.SharedInt,
                            index = (ulong)get_shared_int_index(shared_int.name),
                        };
                    } else if (external is ContextSharedFloat shared_float) {
                        e = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal {
                            et = AutoCode.Packets.BehaviourFlowExports.ExternalType.SharedFloat,
                            index = (ulong)get_shared_float_index(shared_float.name),
                        };
                    } else if (external is ExpressionExternal ee) {
                        e = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal {
                            et = AutoCode.Packets.BehaviourFlowExports.ExternalType.External,
                            index = (ulong)get_string_index(ee.name),
                        };
                    } else {
                        throw new System.InvalidCastException();
                    }
                    _externals.items[i] = e;
                }
            } else {
                _externals.items = new AutoCode.Packets.BehaviourFlowExports.ExpressionExternal[0];
            }
            var _functions = new Foundation.Packets.List<Foundation.Packets.cuint>();
            if (functions != null) {
                _functions.items = new Foundation.Packets.cuint[functions.Length];
                for (int i = 0; i < functions.Length; ++i) {
                    _functions.items[i] = (ulong)get_string_index(functions[i].name);
                }
            } else {
                _functions.items = new Foundation.Packets.cuint[0];
            }
            m_expressions.Add(new AutoCode.Packets.BehaviourFlowExports.Expression {
                byte_code = byte_code,
                externals = _externals,
                functions = _functions,
            });
            return index;
        }

        public int add_node(Foundation.Packets.IPacket node) {
            var index = m_nodes.Count;
            m_nodes.Add(node);
            return index;
        }

        public void save_to(System.IO.BinaryWriter writer) {
            var head = new AutoCode.Packets.BehaviourFlowExports.Head();
            head.sign.items = new Foundation.Packets.u8[] { (byte)'V', (byte)'N', (byte)'B', (byte)'T' };
            head.version = 1;
            head.save_to(writer);

            var body = new AutoCode.Packets.BehaviourFlowExports.Body();
            body.strings.items = new Foundation.Packets.String[m_strings.Count];
            foreach (var kvp in m_strings) {
                body.strings.items[kvp.Value] = kvp.Key;
            }
            body.shared_ints.items = new Foundation.Packets.cuint[m_shared_ints.Count];
            foreach (var kvp in m_shared_ints) {
                var value = kvp.Value;
                body.shared_ints.items[value.index] = (ulong)value.name_index;
            }
            body.shared_floats.items = new Foundation.Packets.cuint[m_shared_floats.Count];
            foreach (var kvp in m_shared_floats) {
                var value = kvp.Value;
                body.shared_floats.items[value.index] = (ulong)value.name_index;
            }
            body.constants.items = new Foundation.Packets.u32[m_constants.Count];
            foreach (var kvp in m_constants) {
                body.constants.items[kvp.Value] = kvp.Key;
            }
            body.expressions.items = m_expressions.ToArray();
            body.node_count = (ulong)m_nodes.Count;
            body.save_to(writer);

            foreach (var node in m_nodes) {
                Foundation.Packets.cuint pid = node.pid;
                pid.save_to(writer);
                node.save_to(writer);
            }
        }

        Dictionary<uint, int> m_constants = new Dictionary<uint, int>();
        Dictionary<string, int> m_strings = new Dictionary<string, int>();
        Dictionary<string, (int index, int name_index)> m_shared_ints = new Dictionary<string, (int index, int name_index)>();
        Dictionary<string, (int index, int name_index)> m_shared_floats = new Dictionary<string, (int index, int name_index)>();
        List<Foundation.Packets.IPacket> m_nodes = new List<Foundation.Packets.IPacket>();
        List<AutoCode.Packets.BehaviourFlowExports.Expression> m_expressions = new List<AutoCode.Packets.BehaviourFlowExports.Expression>();
    }
}