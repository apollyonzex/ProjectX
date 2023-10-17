

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Worlds.Missions.Dialogs {
    public class MissionDialogOption : MonoBehaviour, ICancelHandler {

        public Text content;
        public Button button;


        public void init(MissionDialogWindow window, int index, string content, bool enabled) {
            m_window = window;
            m_index = index;
            if (this.content != null) {
                this.content.text = content;
            }
            if (button != null) {
                if (enabled) {
                    button.interactable = true;
                    button.targetGraphic.raycastTarget = true;
                    button.onClick.AddListener(() => {
                        m_window.current_page.do_option(window, index);
                    });
                } else {
                    button.interactable = false;
                    button.targetGraphic.raycastTarget = false;
                }
            }

        }

        void ICancelHandler.OnCancel(BaseEventData eventData) {
            if (m_window != null) {
                m_window.close();
            }
        }

        private MissionDialogWindow m_window;
        private int m_index;
        
    }
}
