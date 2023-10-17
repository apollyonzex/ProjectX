

using System;

namespace GraphNode {
    public abstract class TypeFilter {
        public abstract bool check(Type type);
    }

    public class WithVoid<T> : TypeFilter where T : TypeFilter, new() {
        public override bool check(Type type) {
            if (type == typeof(void)) {
                return true;
            }
            return new T().check(type);
        }
    }

    public class TypeTuple<T> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T).IsAssignableFrom(type);
        }
    }

    public class TypeTuple<T1, T2> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type) || typeof(T2).IsAssignableFrom(type);
        }
    }

    public class TypeTuple<T1, T2, T3> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type) 
                || typeof(T3).IsAssignableFrom(type)
                ;
        }
    }

    public class TypeTuple<T1, T2, T3, T4> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type)
                || typeof(T3).IsAssignableFrom(type)
                || typeof(T4).IsAssignableFrom(type)
                ;
        }
    }

    public class TypeTuple<T1, T2, T3, T4, T5> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type)
                || typeof(T3).IsAssignableFrom(type)
                || typeof(T4).IsAssignableFrom(type)
                || typeof(T5).IsAssignableFrom(type)
                ;
        }
    }

    public class TypeTuple<T1, T2, T3, T4, T5, T6> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type)
                || typeof(T3).IsAssignableFrom(type)
                || typeof(T4).IsAssignableFrom(type)
                || typeof(T5).IsAssignableFrom(type)
                || typeof(T6).IsAssignableFrom(type)
                ;
        }
    }

    public class TypeTuple<T1, T2, T3, T4, T5, T6, T7> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type)
                || typeof(T3).IsAssignableFrom(type)
                || typeof(T4).IsAssignableFrom(type)
                || typeof(T5).IsAssignableFrom(type)
                || typeof(T6).IsAssignableFrom(type)
                || typeof(T7).IsAssignableFrom(type)
                ;
        }
    }

    public class TypeTuple<T1, T2, T3, T4, T5, T6, T7, T8> : TypeFilter {
        public override bool check(Type type) {
            return typeof(T1).IsAssignableFrom(type)
                || typeof(T2).IsAssignableFrom(type)
                || typeof(T3).IsAssignableFrom(type)
                || typeof(T4).IsAssignableFrom(type)
                || typeof(T5).IsAssignableFrom(type)
                || typeof(T6).IsAssignableFrom(type)
                || typeof(T7).IsAssignableFrom(type)
                || typeof(T8).IsAssignableFrom(type)
                ;
        }
    }
}