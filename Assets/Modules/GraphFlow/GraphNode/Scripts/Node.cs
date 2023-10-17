
using System.Collections.Generic;
using UnityEngine;

namespace GraphNode {


    [System.Serializable]
    public struct UnityObjectRef<T> where T : Object {
        [System.NonSerialized]
        public T obj;

        public int index;

        public void before_serialize(List<Object> referenced_objects) {
            if (obj == null) {
                index = -1;
            } else {
                index = referenced_objects.Count;
                referenced_objects.Add(obj);
            }
        }

        public void after_deserialize(Object[] referenced_objects) {
            if (index != -1) {
                obj = referenced_objects[index] as T;
            }
        }
    }

    [System.Serializable]
    public abstract class Node {

        public Vector2 position {
            get => new Vector2(m_x, m_y);
            set {
                m_x = value.x;
                m_y = value.y;
            }
        }
        private float m_x, m_y;

        [SortedOrder(-99)][Display("Comment")]
        public Comment comment;

        public virtual void on_serializing(List<Object> referenced_objects) { }
        public virtual void on_deserialized(Object[] referenced_objects) { }
    }

    [System.Serializable]
    public struct Comment : System.IEquatable<Comment> {
        public string content;

        public override int GetHashCode() {
            return (content ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is Comment other && Equals(other);
        }

        public bool Equals(Comment other) {
            return content == other.content;
        }

        public override string ToString() {
            return content?.ToString();
        }
    }

    [System.Serializable]
    public struct LongString : System.IEquatable<LongString> {
        public string content;

        public override int GetHashCode() {
            return (content ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj) {
            return obj is LongString other && Equals(other);
        }

        public bool Equals(LongString other) {
            return content == other.content;
        }

        public override string ToString() {
            return content?.ToString();
        }
    }

}