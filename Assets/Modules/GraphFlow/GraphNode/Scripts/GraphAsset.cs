
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

namespace GraphNode {

    public abstract class GraphAsset : ScriptableObject {

        public abstract Graph new_graph();

        public virtual bool save_graph(Graph graph) {
            var referenced_objects = new List<Object>();
            graph.before_serialize(this, referenced_objects);
            try {
                using (var stream = new MemoryStream()) {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, graph);
                    m_data = stream.ToArray();
                    m_referenced_objects = referenced_objects.ToArray();
                    if (m_graph != null) {
                        m_graph = graph;
                    }
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
#endif
                }
            } catch (System.Exception e) {
                Debug.LogError(e);
                return false;
            }
            return true;
        }

        public virtual Graph load_graph() {
            if (m_data == null || m_data.Length == 0) {
                return new_graph();
            }
            Graph graph;
            try {
                using (var stream = new MemoryStream(m_data)) {
                    var formatter = new BinaryFormatter();
                    graph = formatter.Deserialize(stream) as Graph;
                }
            } catch (System.Exception e) {
                Debug.LogError(e);
                return null;
            }
            if (graph != null) {
                graph.after_deserialize(this, m_referenced_objects);
            }
            return graph;
        }

        protected void OnEnable() {
            m_graph = null;
        }

        protected void OnDisable() {
            m_graph = null;
        }

        public T get_graph<T>() where T : Graph {
            if (m_graph == null) {
                m_graph = load_graph() ?? new_graph();
            }
            return m_graph as T;
        }

        public T get_graph_unchecked<T>() where T : Graph {
            return m_graph as T;
        }


        [SerializeField][HideInInspector]
        private byte[] m_data;


        [SerializeField][HideInInspector]
        private Object[] m_referenced_objects;

        Graph m_graph;
    }

    public abstract class GraphAsset<T> : GraphAsset where T : Graph {
        public T graph => get_graph<T>();
        public T graph_unchecked => get_graph_unchecked<T>();
    }
    
}