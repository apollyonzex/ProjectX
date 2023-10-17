
using System.Collections.Generic;

namespace InvokeFlow {

    public enum VariableType {
        Integer = 1,
        Floating = 2,
        Boolean = 3,
    }

    [System.Serializable]
    public abstract class Variable {
        public abstract VariableType type { get; }
        public string name;

        public abstract Variable clone();

        public abstract string value_string { get; }

        public abstract int value_in_stack { get; }
    }

    [System.Serializable]
    public class VariableInt : Variable {
        public override VariableType type => VariableType.Integer;
        public VariableInt() {

        }
        public VariableInt(string name) {
            this.name = name;
        }
        public int value;

        public override Variable clone() => new VariableInt() { name = name, value = value };
        public override string value_string => value.ToString();
        public override int value_in_stack => value;
    }

    [System.Serializable]
    public class VariableFloat : Variable {

        public VariableFloat() {
        
        }

        public VariableFloat(string name) {
            this.name = name;
        }

        public override VariableType type => VariableType.Floating;

        public float value;

        public override Variable clone() => new VariableFloat() { name = name, value = value };
        public override string value_string => value.ToString();

        public override int value_in_stack => (int)CalcExpr.Utility.convert_to(value);
    }


    [System.Serializable]
    public class VariableBool : Variable {

        public VariableBool() {

        }

        public VariableBool(string name) {
            this.name = name;
        }

        public override VariableType type => VariableType.Boolean;

        public bool value;

        public override Variable clone() => new VariableBool() { name = name, value = value };
        public override string value_string => value ? "true" : "false";

        public override int value_in_stack => value ? 1 : 0;
    }

    [System.Serializable]
    public class Variables : List<Variable> {
        public Variables() {
            
        }

        public Variables(int capacity) : base(capacity) {

        }

        public Variables clone() {
            var ret = new Variables(Count);
            foreach (var item in this) {
                ret.Add(item.clone());
            }
            return ret;
        }

        public IEnumerator<Variable> enumerate_valid_variables() {
            for (int i = 0; i < Count; ++i) {
                var vi = this[i];
                if (string.IsNullOrEmpty(vi.name)) {
                    continue;
                }
                var ok = true;
                for (int j = i + 1; j < Count; ++j) {
                    var vj = this[j];
                    if (vi.name == vj.name) {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return vi;
                }
            }
        }
    }

    [System.Serializable]
    public class Parameter : Variable {
        public override VariableType type => m_type;

        public override string value_string => null;

        public override int value_in_stack => 0;

        public void change_type(VariableType type) {
            m_type = type;
        }

        public override Variable clone() {
            return new Parameter(m_type, name);
        }

        public Parameter(VariableType type, string name) {
            m_type = type;
            this.name = name;
        }

        VariableType m_type;
    }

    [System.Serializable]
    public class Parameters : List<Parameter> {
        public Parameters() {

        }

        public Parameters(int capacity) : base(capacity) {

        }

        public Parameters clone() {
            var ret = new Parameters(Count);
            foreach (var p in this) {
                ret.Add(new Parameter(p.type, p.name));
            }
            return ret;
        }

        public IEnumerator<Variable> enumerate_valid_variables() {
            for (int i = 0; i < Count; ++i) {
                var vi = this[i];
                if (string.IsNullOrEmpty(vi.name)) {
                    continue;
                }
                var ok = true;
                for (int j = i + 1; j < Count; ++j) {
                    var vj = this[j];
                    if (vi.name == vj.name) {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    yield return vi;
                }
            }
        }
    }
}