

using DialogFlow;
using Inits;

namespace Worlds.Missions.Dialogs {

    [System.Serializable]
    public class MissionDialogGraph : DialogGraph {
        public override System.Type context_type => typeof(IMissionDialogContext);

        public static string translate(string content_id) {
            content_id = content_id.TrimEnd('\r', '\n');
            var content = LanguageMgr.instance.get_text(content_id);

            //临时：配置不全
            if (content == "")
                content = content_id;

            return content;
        }
    }
}
