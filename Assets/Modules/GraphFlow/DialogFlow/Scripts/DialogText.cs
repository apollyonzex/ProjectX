
using System.Collections.Generic;
using GraphNode;

namespace DialogFlow {

    [System.Serializable]
    public class DialogText {

        public string content {
            get => _content.content;
            set => _content.content = value;
        }

        public DialogText clone() {
            var ret = new DialogText {
                _content = _content,
            };
            foreach (var p in parameters) {
                ret.parameters.Add(new Parameter {
                    type = p.type,
                    value = p.value?.clone() as Expression,
                });
            }
            return ret;
        }
        

        [System.Serializable]
        public class Parameter {
            public ActionParameterType type;
            public Expression value;
        }

        public List<Parameter> parameters = new List<Parameter>();

        public static implicit operator DialogText(LongString value) {
            return new DialogText { _content = value };
        }

        public LongString _content;

        public void show_text(IContext context) {
            if (parameters.Count == 0) {
                context.show_text(_content.content ?? string.Empty);
            } else {
                context.show_text(_content.content ?? string.Empty, build_arguments(context));
            }
        }

        public void show_option(IContext context, int index, bool enabled) {
            if (parameters.Count == 0) {
                context.show_option(index, enabled, _content.content ?? string.Empty);
            } else {
                context.show_option(index, enabled, _content.content ?? string.Empty, build_arguments(context));
            }
        }

        private object[] build_arguments(IContext context) {
            var args = new object[parameters.Count];
            for (int i = 0; i < args.Length; ++i) {
                var p = parameters[i];
                switch (p.type) {
                    case ActionParameterType.Content:
                        args[i] = context.translate_argument(p.value.content);
                        break;
                    case ActionParameterType.Integer: {
                        if (p.value.calc(context, context.context_type, out int val)) {
                            args[i] = val;
                        }
                        break;
                    }
                    case ActionParameterType.Floating: {
                        if (p.value.calc(context, context.context_type, out float val)) {
                            args[i] = val;
                        }
                        break;
                    }
                    case ActionParameterType.Boolean: {
                        if (p.value.calc(context, context.context_type, out bool val)) {
                            args[i] = val;
                        }
                        break;
                    }
                }
            }
            return args;
        }
    }
}