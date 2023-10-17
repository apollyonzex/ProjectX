using AutoCode.Tables;
using UnityEngine;
using World_Formal.DS;

namespace World_Formal.Caravans
{
    public class Caravan : ITarget
    {
        public Body.Record _desc;

        public Vector2 colider_size;
        public string body_prefab_path;
        public string body_path;

        public float wheel_height;
        public float wheel_to_caravan_dis;
        public Vector2 body_spine_offset;

        public float height_offset => wheel_height + wheel_to_caravan_dis;

        Vector2 DS.ITarget.Position { 
            get
            {
                return WorldContext.instance.caravan_pos;
            } 
        }

        Collider2D ITarget.collider => mgr.collider;

        public CaravanMgr_Formal mgr;

        //==================================================================================================

        /// <summary>
        /// 用于初始化的构造函数
        /// </summary>
        public Caravan(Body.Record r, CaravanEnhanced.CaravanData asset)
        {
            _desc = r;

            body_path = asset.body_path;
            body_prefab_path = asset.body_prefab_path;
            colider_size = asset.size;
        }

        /// <summary>
        /// 
        /// </summary>
        public Caravan(Body.Record r)
        {
            _desc = r;
        }


        void DS.ITarget.hurt(int dmg)
        {
            hurt(dmg);
        }


        public void hurt(int dmg)
        {
            WorldContext.instance.caravan_hp -= dmg;
        }
    }
}

