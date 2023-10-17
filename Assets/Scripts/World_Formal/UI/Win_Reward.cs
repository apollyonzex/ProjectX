using Common_Formal;
using UnityEngine;
using World_Formal.CaravanBackPack;
using World_Formal.Helpers;
using World_Formal.WareHouse;

namespace World_Formal.UI
{
    public class Win_Reward : MonoBehaviour
    {
        public DeviceWareHouseView wview;
        public Transform backpack_node;

        WorldSceneRoot root;
        Win_BackPack_World m_win_backpack;
        Transform m_tsf_backpack;
        DeviceWareHouseManager mgr;

        Vector3 m_win_backpack_pos_offset;

        //==================================================================================================

        public void init(WorldSceneRoot root, Change_Device_Helper helper)
        {
            this.root = root;
            m_win_backpack = root.win_backpack;
            m_tsf_backpack = m_win_backpack.transform;
            m_win_backpack_pos_offset = backpack_node.transform.localPosition;

            //奖励区
            var name = Common.Config.RewardHouse_Name;
            mgr = new DeviceWareHouseManager(name);
            mgr.init_warehouse();
            Mission.instance.attach_mgr(name, mgr);

            mgr.add_view(wview);
            mgr.init_helper(helper);
        }


        public void _active(bool bl)
        {
            gameObject.SetActive(bl);
        }


        private void OnEnable()
        {
            if (root == null) return;

            m_tsf_backpack.localPosition = m_win_backpack_pos_offset;
            m_win_backpack.btn_close.gameObject.SetActive(false);
            m_win_backpack._active(true);
        }


        private void OnDisable()
        {
            if (root == null) return;

            m_tsf_backpack.localPosition = Vector3.zero;
            m_win_backpack.btn_close.gameObject.SetActive(true);
        }


        /// <summary>
        /// 确认，关闭win
        /// </summary>
        public void btn_comfirm()
        {
            gameObject.SetActive(false);
            m_win_backpack._active(false);

            mgr.remove_all_devices();
        }


        /// <summary>
        /// 入口，激活win
        /// </summary>
        public void _active()
        {
            if (!Battle_Reward_Helper.instance.try_create_reward(out var rewards)) return;
            foreach (var reward in rewards)
            {
                DB.instance.device.try_get(reward.Item1, reward.Item2, out AutoCode.Tables.Device.Record device_record);
                mgr.put_device(new Caravans.Devices.Device(device_record.f_id));
            }

            gameObject.SetActive(true);
        }
    }
}

