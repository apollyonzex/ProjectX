using System;
using System.Runtime.Serialization;

namespace Foundation {

    [Serializable]
    public class SystemTypeWrap : ISerializable, IEquatable<SystemTypeWrap>, IEquatable<Type> {

        public SystemTypeWrap() {

        }

        public SystemTypeWrap(Type val) {
            this.val = val;
        }

        public SystemTypeWrap(SerializationInfo info, StreamingContext context) {
            var name = (string)info.GetValue(string.Empty, typeof(string));
            if (name != null) {
                val = Type.GetType(name, false);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            string name = null;
            if (val != null) {
                name = $"{val.FullName}, {val.Assembly.GetName().Name}";
            }
            info.AddValue(string.Empty, name, typeof(string));
        }

        public Type val;

        public override int GetHashCode() {
            return val != null ? val.GetHashCode() : 0;
        }

        public override bool Equals(object obj) {
            return obj is SystemTypeWrap other && other.val == val;
        }

        public bool Equals(SystemTypeWrap other) {
            return val == other?.val;
        }

        public bool Equals(Type other) {
            return val == other;
        }

        public static bool operator ==(SystemTypeWrap a, SystemTypeWrap b) {
            return a?.val == b?.val;
        }

        public static bool operator !=(SystemTypeWrap a, SystemTypeWrap b) {
            return a?.val != b?.val;
        }
    }
}