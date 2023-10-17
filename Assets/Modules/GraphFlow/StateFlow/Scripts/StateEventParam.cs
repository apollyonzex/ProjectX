
using CalcExpr;

namespace StateFlow {
    public struct StateEventParam {
        public int stack_value;
        public IStateEventParamRef ref_val;

        public static implicit operator StateEventParam(int value) {
            return new StateEventParam { stack_value = value };
        }

        public static implicit operator StateEventParam(float value) {
            return new StateEventParam { stack_value = (int)Utility.convert_to(value) };
        }

        public static implicit operator StateEventParam(bool value) {
            return new StateEventParam { stack_value = value ? 1 : 0 };
        }

        public StateEventParam(IStateEventParamRef ref_val) {
            stack_value = ref_val.stack_value;
            this.ref_val = ref_val;
        }
    }

    public interface IStateEventParamRef {
        int stack_value { get; set; }
    }

    public class StateEventParamInt : IStateEventParamRef {
        public int value;
        int IStateEventParamRef.stack_value {
            get => value;
            set => this.value = value;
        }

        public StateEventParamInt(int value) {
            this.value = value;
        }

        public static implicit operator StateEventParamInt(int value) {
            return new StateEventParamInt(value);
        }

        public static implicit operator StateEventParam(StateEventParamInt obj) {
            return new StateEventParam(obj);
        }
    }

    public class StateEventParamFloat : IStateEventParamRef {
        public float value;
        int IStateEventParamRef.stack_value {
            get => (int)Utility.convert_to(value);
            set => this.value = Utility.convert_float_from((uint)value);
        }

        public StateEventParamFloat(float value) {
            this.value = value;
        }

        public static implicit operator StateEventParamFloat(float value) {
            return new StateEventParamFloat(value);
        }

        public static implicit operator StateEventParam(StateEventParamFloat obj) {
            return new StateEventParam(obj);
        }
    }

    public class StateEventParamBool : IStateEventParamRef {
        public bool value;
        int IStateEventParamRef.stack_value {
            get => value ? 1 : 0;
            set => this.value = value != 0 ? true : false;
        }

        StateEventParamBool(bool value) {
            this.value = value;
        }

        public static implicit operator StateEventParamBool(bool value) {
            return new StateEventParamBool(value);
        }

        public static implicit operator StateEventParam(StateEventParamBool obj) {
            return new StateEventParam(obj);
        }
    }
}