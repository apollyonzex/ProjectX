

using BehaviourFlow.Exports;
using GraphNode;
using System.Collections.Generic;

namespace BehaviourFlow.Nodes {
    [System.Serializable]
    [Graph(typeof(BehaviourTree))]
    public class ParallelNode : BTChildNodeWithChildren {

        public enum Mode {
            First,
            Last,
            First_Success,
            First_Failed,
        }

        [Display("Mode")]
        [ShowInBody]
        public Mode mode;

        protected override IEnumerator<BTResult> _exec(BTExecutorBase executor) {
            if (children.Count == 0) {
                return null;
            }
            switch (mode) {
                case Mode.First: return _first(executor);
                case Mode.Last: return _last(executor);
                case Mode.First_Success: return _first_success(executor);
                case Mode.First_Failed: return _first_failed(executor);
            }
            return null;
        }

        private IEnumerator<BTResult> _first(BTExecutorBase executor) {
            var execs = create_executors(executor);
            var asset = executor.last_asset;
            for (int i = 0; i < execs.Length; ++i) {
                var child = children[i].value;
                var e = execs[i];
                if (e.reset(child, asset, true)) {
                    var ret = e.last_result;
                    cleanup(execs);
                    if (ret) {
                        yield return BTResult.success;
                    }
                    yield break;
                }
            }
            executor.on_abort(() => cleanup(execs));
        l_again:
            yield return BTResult.pending;
            for (int i = 0; i < execs.Length; ++i) {
                var e = execs[i];
                if (e.exec()) {
                    var ret = e.last_result;
                    cleanup(execs);
                    if (ret) {
                        yield return BTResult.success;
                    }
                    yield break;
                }
            }
            goto l_again;
        }

        private IEnumerator<BTResult> _last(BTExecutorBase executor) {
            var execs = create_executors(executor);
            var asset = executor.last_asset;
            var last_ret = false;
            for (int i = 0; i < execs.Length; ++i) {
                var child = children[i].value;
                var e = execs[i];
                if (e.reset(child, asset, true)) {
                    last_ret = e.last_result;
                    e.dispose();
                    execs[i] = null;
                }
            }
            var done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                if (last_ret) {
                    yield return BTResult.success;
                }
                yield break;
            }
            executor.on_abort(() => cleanup(execs));
        l_again:
            yield return BTResult.pending;
            for (int i = 0; i < execs.Length; ++i) {
                var e = execs[i];
                if (e != null && e.exec()) {
                    last_ret = e.last_result;
                    e.dispose();
                    execs[i] = null;
                }
            }
            done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                if (last_ret) {
                    yield return BTResult.success;
                }
                yield break;
            }
            goto l_again;
        }

        private IEnumerator<BTResult> _first_success(BTExecutorBase executor) {
            var execs = create_executors(executor);
            var asset = executor.last_asset;
            for (int i = 0; i < execs.Length; ++i) {
                var child = children[i].value;
                var e = execs[i];
                if (e.reset(child, asset, true)) {
                    if (e.last_result) {
                        cleanup(execs);
                        yield return BTResult.success;
                        yield break;
                    }
                    e.dispose();
                    execs[i] = null;
                }
            }
            var done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                yield break;
            }
            executor.on_abort(() => cleanup(execs));
        l_again:
            yield return BTResult.pending;
            for (int i = 0; i < execs.Length; ++i) {
                var e = execs[i];
                if (e != null && e.exec()) {
                    if (e.last_result) {
                        cleanup(execs);
                        yield return BTResult.success;
                        yield break;
                    }
                    e.dispose();
                    execs[i] = null;
                }
            }
            done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                yield break;
            }
            goto l_again;
        }

        private IEnumerator<BTResult> _first_failed(BTExecutorBase executor) {
            var execs = create_executors(executor);
            var asset = executor.last_asset;
            for (int i = 0; i < execs.Length; ++i) {
                var child = children[i].value;
                var e = execs[i];
                if (e.reset(child, asset, true)) {
                    if (!e.last_result) {
                        cleanup(execs);
                        yield break;
                    }
                    e.dispose();
                    execs[i] = null;
                }
            }
            var done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                yield return BTResult.success;
                yield break;
            }
            executor.on_abort(() => cleanup(execs));
        l_again:
            yield return BTResult.pending;
            for (int i = 0; i < execs.Length; ++i) {
                var e = execs[i];
                if (e != null && e.exec()) {
                    if (!e.last_result) {
                        cleanup(execs);
                        yield break;
                    }
                    e.dispose();
                    execs[i] = null;
                }
            }
            done = true;
            foreach (var e in execs) {
                if (e != null) {
                    done = false;
                    break;
                }
            }
            if (done) {
                yield return BTResult.success;
                yield break;
            }
            goto l_again;
        }

        private BTChildExecutor[] create_executors(BTExecutorBase parent) {
            var executors = new BTChildExecutor[children.Count];
            foreach (ref var e in Foundation.ArraySlice.create(executors)) {
                e = new BTChildExecutor(parent);
#if UNITY_EDITOR
                e.debug_info("parallel", parent.game_object);
#endif
            }
            return executors;
        }

        private void cleanup(BTChildExecutor[] execs) {
            foreach (var e in execs) {
                e?.dispose();
            }
        }

        public override bool export(Exporter exporter, out int index) {
            var node = new AutoCode.Packets.BehaviourFlowExports.Parallel();
            switch (mode) {
                default:
                case Mode.First:
                    node.mode = 0;
                    break;
                case Mode.Last:
                    node.mode = 1;
                    break;
                case Mode.First_Success:
                    node.mode = 2;
                    break;
                case Mode.First_Failed:
                    node.mode = 3;
                    break;
            }
            var _children = new List<Foundation.Packets.cuint>(children.Count);
            foreach (var child in children) {
                if (child.value != null) {
                    if (child.value.export(exporter, out var child_index)) {
                        _children.Add((ulong)child_index);
                    }
                }
            }
            node.children.items = _children.ToArray();
            index = exporter.add_node(node);
            return true;
        }
    }
}