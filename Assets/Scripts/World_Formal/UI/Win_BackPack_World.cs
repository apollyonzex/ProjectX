using UnityEngine;
using UnityEngine.UI;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.Helpers;

namespace World_Formal.UI
{
    public class Win_BackPack_World : MonoBehaviour
    {
        public BackPackView bview;
        public Transform content;
        public Button btn_close;

        WorldSceneRoot root;

        //==================================================================================================

        public void init(WorldSceneRoot root, CaravanMgr_Formal cvMgr, Change_Device_Helper helper)
        {
            this.root = root;

            BackPack_Helper.instance.init(bview, content, cvMgr, helper);
        }


        public void _active(bool bl)
        {
            gameObject.SetActive(bl);
            root.texture_camara.gameObject.SetActive(bl);
        }
    }
}

