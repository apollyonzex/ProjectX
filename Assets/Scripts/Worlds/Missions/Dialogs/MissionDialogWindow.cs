using Assets.Scripts.World_Formal.Dialog_Formal;
using Common;
using DialogFlow;
using DialogFlow.Nodes;
using Foundation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Worlds.Missions.Dialogs.Nodes;


namespace Worlds.Missions.Dialogs
{

    public interface IMissionDialogHandler
    {
        void end();
        void destroy();
        void enter_normal_battle(System.Action<bool> callback);
    }

    public class MissionDialogWindow : MonoBehaviour, IMissionDialogContext, ISubmitHandler, IPointerClickHandler
    {
        public Transform options;
        public UnityEngine.UI.Text content;
        public MissionDialogOption optionPrefab;
        public UnityEngine.UI.Slider continueTimer;
        public UnityEngine.UI.Text title;
        public UnityEngine.UI.Image image;

        public PageNodeBase current_page { get; private set; }
        private IMissionDialogHandler m_handler;
        private List<MissionDialogOption> m_options = new List<MissionDialogOption>();
        private bool m_has_continue = false;
        private float m_continue_time = 0;
        private float m_continue_timer = 0;
        private Stack<JumpToDialog> m_nodes = new Stack<JumpToDialog>();
        private Stack<Waiting> m_waitings = new Stack<Waiting>();
        private bool m_module_running = false;
        private string m_title;

        public void init(IMissionDialogHandler handler, string title,Sprite s = null)
        {
            m_handler = handler;
            m_title = MissionDialogGraph.translate(title);
            image.sprite = s;
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void close()
        {
            if (current_page == null)
            {
                Destroy(gameObject);
                m_handler?.end();
            }
            else if (m_has_continue)
            {
                current_page.do_continue(this);
            }
        }

        private void Update()
        {
            if (m_continue_time > 0 && m_continue_timer > 0)
            {
                m_continue_timer -= Time.deltaTime;
                if (m_continue_timer <= 0)
                {
                    if (continueTimer != null)
                    {
                        continueTimer.normalizedValue = 0;
                    }
                    current_page.do_continue(this);
                }
                else if (continueTimer != null)
                {
                    continueTimer.normalizedValue = m_continue_timer / m_continue_time;
                }
            }
        }

        private void on_module_ended(bool result)
        {
            m_module_running = false;
            if (m_waitings.Count != 0)
            {
                gameObject.SetActive(true);

                var node = m_waitings.Pop();
                var action = result ? node.success : node.failed;
                if (action != null)
                {
                    action.Invoke(this);
                }
                else
                {
                    ((IContext)this).end();
                }
            }
        }

        private void on_module_failed()
        {
            m_module_running = false;
            if (m_waitings.Count != 0)
            {
                var action = m_waitings.Pop().failed;
                if (action != null)
                {
                    action.Invoke(this);
                }
                else
                {
                    ((IContext)this).end();
                }
            }
        }

        private bool try_start_module(string name)
        {
            if (m_module_running)
            {
                Debug.LogError($"start ${name} failed: another module is running");
                return false;
            }
            if (m_waitings.Count == 0)
            {
                Debug.LogError($"start ${name} failed: need waiting node");
                return false;
            }
            m_module_running = true;
            return true;
        }


        System.Type GraphNode.IContext.context_type => typeof(IMissionDialogContext);

        int IMissionDialogContext.hp_of_caravan_body => 1000;


        void IContext.end()
        {
            if (m_waitings.Count == 0)
            {
                Destroy(gameObject);
                m_handler?.end();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        void IContext.jump_to(PageNodeBase page)
        {
            foreach (var op in m_options)
            {
                Destroy(op.gameObject);
            }
            m_options.Clear();
            m_has_continue = false;
            m_continue_time = 0;
            m_continue_timer = 0;
            if (continueTimer != null)
            {
                continueTimer.gameObject.SetActive(false);
            }

            current_page = page;

            page.show(this);

            bool has_available_option = false;
            foreach (var op in m_options)
            {
                if (op.button != null && op.button.interactable)
                {
                    has_available_option = true;
                    EventSystem.current.SetSelectedGameObject(op.gameObject);
                    break;
                }
            }
            if (!has_available_option)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (m_has_continue && m_continue_time == 0)
            {
                current_page.do_continue(this);
            }
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            if (m_has_continue)
            {
                current_page.do_continue(this);
            }
        }

        JumpToDialog IContext.peek_externals()
        {
            if (m_nodes.Count > 0)
            {
                return m_nodes.Peek();
            }
            return null;
        }

        JumpToDialog IContext.pop_externals()
        {
            if (m_nodes.Count > 0)
            {
                return m_nodes.Pop();
            }
            return null;
        }

        void IContext.push_externals(JumpToDialog node)
        {
            m_nodes.Push(node);
        }

        void IContext.show_continue(float timeout)
        {
            m_has_continue = true;
            m_continue_time = timeout;

            if (m_continue_time > 0)
            {
                m_continue_timer = timeout;
                if (continueTimer != null)
                {
                    continueTimer.normalizedValue = 1;
                    continueTimer.gameObject.SetActive(true);
                }
            }
        }

        void IContext.show_option(int index, bool enabled, string content)
        {
            if (optionPrefab != null)
            {
                var op = Instantiate(optionPrefab, options);
                op.init(this, index, MissionDialogGraph.translate(content), enabled);
                op.gameObject.SetActive(true);
                m_options.Add(op);
            }
        }

        void IContext.show_option(int index, bool enabled, string format, object[] arguments)
        {
            if (optionPrefab != null)
            {
                var op = Instantiate(optionPrefab, options);
                op.init(this, index, string.Format(MissionDialogGraph.translate(format), arguments), enabled);
                op.gameObject.SetActive(true);
                m_options.Add(op);
            }
        }

        void IContext.show_text(string content)
        {
            if (this.content != null)
            {

                string str = MissionDialogGraph.translate(content);
                this.title.text = m_title;
                this.content.text = str;
            }
        }

        void IContext.show_text(string format, object[] arguments)
        {
            if (content != null)
            {
                content.text = string.Format(MissionDialogGraph.translate(format), arguments);
            }
        }

        string IContext.translate_argument(string content)
        {
            return MissionDialogGraph.translate(content);
        }

        void IMissionDialogContext.push_waiting(Waiting node)
        {
            m_waitings.Push(node);
        }

        void IMissionDialogContext.enter_normal_battle()
        {
            if (try_start_module("normal_battle")) 
            {
                m_handler?.enter_normal_battle(on_module_ended);
            }            
        }


        void IMissionDialogContext.enter_elite_battle()
        {
        }


        void IMissionDialogContext.destroy()
        {
            on_module_ended(true);
        }


        void IMissionDialogContext.end_level_victory()
        {
            (this as IMissionDialogContext).destroy();
            Debug.Log("关卡胜利");
        }


        void IMissionDialogContext.end_level_fail()
        {
            (this as IMissionDialogContext).destroy();
            Debug.Log("关卡失败");
        }


        void IMissionDialogContext.get_random_device(int pool_id, int item_num, int loop_times, float hp_remaining)
        {
            /*if (try_start_module("get_random_device"))
            {
                var win = WorldSceneRoot.instance.Open_UI_Prefab<Get_Random_Device_Win>("encounter", "get_random_device_win");
                if (win != null)
                {
                    win.callback += () => { on_module_ended(true); };
                }

            }*/
        }


        /// <summary>
        /// 根据权重做随机分支判断
        /// </summary>
        /// <param name="w1"></param>
        /// <param name="w2"></param>
        /// <returns></returns>
        int IMissionDialogContext.rnd_2(int w1, int w2)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2);
        }

        int IMissionDialogContext.rnd_3(int w1, int w2, int w3)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3);
        }

        int IMissionDialogContext.rnd_4(int w1, int w2, int w3, int w4)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4);
        }

        int IMissionDialogContext.rnd_5(int w1, int w2, int w3, int w4, int w5)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5);
        }

        int IMissionDialogContext.rnd_6(int w1, int w2, int w3, int w4, int w5, int w6)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5, w6);
        }

        int IMissionDialogContext.rnd_7(int w1, int w2, int w3, int w4, int w5, int w6, int w7)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5, w6, w7);
        }

        int IMissionDialogContext.rnd_8(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5, w6, w7, w8);
        }

        int IMissionDialogContext.rnd_9(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8, int w9)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5, w6, w7, w8, w9);
        }

        int IMissionDialogContext.rnd_10(int w1, int w2, int w3, int w4, int w5, int w6, int w7, int w8, int w9, int w10)
        {
            return Common.Expand.Utility.get_random_index_from_weight(w1, w2, w3, w4, w5, w6, w7, w8, w9, w10);
        }

        void IXContext.show_illustration(string bundle,string path)
        {
            AssetBundleManager.instance.load_asset<Sprite>(bundle, path, out Sprite s);
            if(s!=null)
                image.sprite = s;
        }

    }
}
