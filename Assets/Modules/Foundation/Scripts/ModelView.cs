
using System.Collections.Generic;

namespace Foundation {

    public interface IModel<T, V> where V : class, IModelView<T> {
        bool add_view(V view);
        bool remove_view(V view);
        bool shift_view(V view, IModel<T, V> new_owner);
        void clear_views();
        bool shift_all_views(IModel<T, V> new_owner);
        ReadOnlyList<V> views { get; }
    }

    public interface IModelView<T> {
        void attach(T owner);
        void detach(T owner);
        void shift(T old_owner, T new_owner);
    }

    public class Model<T, V> : IModel<T, V>
        where T : class, IModel<T, V>
        where V : class, IModelView<T> {

        public ReadOnlyList<V> views => m_views;

        public bool add_view(V view) {
            if (m_views.Contains(view)) {
                return false;
            }
            m_views.Add(view);
            view.attach(this as T);
            return true;
        }

        public bool remove_view(V view) {
            var idx = m_views.IndexOf(view);
            if (idx >= 0) {
                var last = m_views.Count - 1;
                if (last != idx) {
                    m_views[idx] = m_views[last];
                }
                m_views.RemoveAt(last);
                view.detach(this as T);
                return true;
            }
            return false;
        }

        public bool shift_view(V view, Model<T, V> new_owner) {
            var idx = m_views.IndexOf(view);
            if (idx < 0) {
                return false;
            }
            m_views.RemoveAt(idx);
            if (new_owner.m_views.Contains(view)) {
                view.detach(this as T);
            } else {
                new_owner.m_views.Add(view);
                view.shift(this as T, new_owner as T);
            }
            return true;
        }

        bool IModel<T, V>.shift_view(V view, IModel<T, V> new_owner) {
            if (new_owner is Model<T, V> obj) {
                return shift_view(view, obj);
            }
            return false;
        }

        public void clear_views() {
            foreach (var view in m_views) {
                view.detach(this as T);
            }
            m_views.Clear();
        }

        public void shift_all_views(Model<T, V> new_owner) {
            foreach (var view in m_views) {
                if (new_owner.m_views.Contains(view)) {
                    view.detach(this as T);
                } else {
                    new_owner.m_views.Add(view);
                    view.shift(this as T, new_owner as T);
                }
            }
            m_views.Clear();
        }

        bool IModel<T, V>.shift_all_views(IModel<T, V> new_owner) {
            if (new_owner is Model<T, V> obj) {
                shift_all_views(obj);
                return true;
            }
            return false;
        }

        private List<V> m_views = new List<V>();
    }
        

}