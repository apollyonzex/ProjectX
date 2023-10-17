using Foundation;
using TMPro;
using UnityEngine;

namespace World.Times
{
    public class TimeView : MonoBehaviour, ITimeView
    {
        public TextMeshProUGUI time;

        TimeMgr mgr;

        //==================================================================================================

        void IModelView<TimeMgr>.attach(TimeMgr mgr)
        {
            this.mgr = mgr;
        }


        void IModelView<TimeMgr>.detach(TimeMgr mgr)
        {
            this.mgr = null;
        }


        void IModelView<TimeMgr>.shift(TimeMgr old_mgr, TimeMgr new_mgr)
        {
        }


        void ITimeView.notify_on_tick1()
        {
            time.text = mgr.view_time;
        }
    }
}

