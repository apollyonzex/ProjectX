using Common_Formal;
using Foundation;
using System.Collections.Generic;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.Caravans.Devices;
using World_Formal.Caravans.Slots;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace World_Formal.CaravanBackPack
{

    /// <summary>
    /// 车体仓库中存储的信息,可以和CaravanMgr_Formal"相互转换"
    /// </summary>
    public class CarBody                                            //  一个车的全部数据应该包括
    { 
        public Caravan caravan;                                     //  1.它主体数据

        public Dictionary<Slot, Device> slot_device_dic = new();    //  2.车体槽位的信息

        public BackPackMgr backPack;                                //  3.背包
    }

    public interface IGarageManagerView   :IModelView<GarageManager>
    {
        void add_car(CarBody car);

        void remove_car(CarBody car);
    }


    public class GarageManager :Model<GarageManager,IGarageManagerView>,IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public GarageManager(string name, params object[]  objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }

        void IMgr.init(params object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);   
        }
        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }

        //==================================================================================================

        public Dictionary<uint, CarBody> cars = new();

        /// <summary>
        /// 通过id添加标准的车体
        /// </summary>
        public void AddCaravan(uint id)
        {
            World_Formal.DB.instance.caravan_body.try_get(id, out var r);
#if UNITY_EDITOR
            if (r == null)
            {
                UnityEngine.Debug.LogWarning("'车体id错误，添加失败");
                return;
            }
#endif
            EX_Utility.try_load_asset(r.f_body_data.Item1, r.f_body_data.Item2, out CaravanEnhanced.CaravanData outter);
            var carbody = new CarBody()
            {
                caravan = new(r, outter),
            };

            EX_Utility.try_load_asset(r.f_backpack.Item1, r.f_backpack.Item2, out CaravanBackPackData data);
            carbody.backPack = new(null,data);

            foreach (var e in outter.slots)
            {
                Slot slot = new(e);
                carbody.slot_device_dic.Add(slot,null);

                Device device = new(e.item_id, slot.type);
                if (device.is_valid)
                {
                    carbody.slot_device_dic[slot] = device;
                    device.type = slot.type;
                }
                carbody.caravan.wheel_height = outter.wheel_height;
                carbody.caravan.wheel_to_caravan_dis = outter.wheel_to_center_dis;
                carbody.caravan.body_spine_offset = outter.body_spine_offset;
            }

            cars.Add(id ,carbody);

            foreach(var view in views)
            {
                view.add_car(carbody);
            }
        }


        public void AddCaravan(CaravanMgr_Formal cmgr)
        {
            var carbody = new CarBody() {
                caravan = cmgr.cell,
                slot_device_dic = cmgr.slot_device_dic,
                backPack = cmgr.backpack,
            };

            cars.Add(cmgr.cell_id, carbody);

            foreach(var view in views)
            {
                view.add_car(carbody);
            }
        }


        public void AddCaravan(uint id, CarBody car)
        {
            World_Formal.DB.instance.caravan_body.try_get(id, out var r);

            EX_Utility.try_load_asset(r.f_backpack.Item1, r.f_backpack.Item2, out CaravanBackPackData data);
            car.backPack = new(null, data);

            cars.Add(id, car);

            foreach (var view in views)
            {
                view.add_car(car);
            }
        }


        public void RemoveCaravan(CarBody car)
        {
            cars.Remove(car.caravan._desc.f_id);

            foreach(var view in views)
            {
                view.remove_car(car);
            }
        }


        public void RemoveCaravan(CaravanMgr_Formal cmgr)
        {
            var id = cmgr.cell_id;
            cars.TryGetValue(id, out var car);
            cars.Remove(id);

            foreach (var view in views)
            {
                view.remove_car(car);
            }
        }


        public void reset()
        {
            foreach (var (_, car) in cars)
            {
                car.backPack = new(null, car.backPack.data);
            }
        }
    }
}
