using AutoCode.Tables;
using Common_Formal;
using Foundation;
using Foundation.Tables;
using System;
using System.Collections.Generic;
using UnityEngine;
using World_Formal.Caravans;
using World_Formal.Enviroment.Road;
using World_Formal.Helpers;

namespace World_Formal.Enviroment
{

    public interface IEnviromentView : IModelView<EnviromentMgr>
    {
        void init_enviorment();

        void update_enviroment();

        void add_obj(float depth, EnviromentObjData data);

        void remove_obj(float depth, EnviromentObjData data);

        void reset_enviroment();
    }
    public class EnviromentMgr : Model<EnviromentMgr, IEnviromentView>, IMgr
    {

        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;


        public EnviromentMgr(string name, params object[] objs)
        {
            m_mgr_name = name;
            (this as IMgr).init(objs);
        }


        void IMgr.destroy()
        {
            Mission.instance.detach_mgr(m_mgr_name);
        }


        void IMgr.init(object[] objs)
        {
            Mission.instance.attach_mgr(m_mgr_name, this);
        }

        //====================================================================================================
        public float dis => m_dis;
        private float m_dis = (Common.Config.current.desiredResolution.x / (float)Common.Config.current.pixelPerUnit) * 2;

        public Vector2 focus_pos;

        public float focus_scale = 1f;

        public Dictionary<float, EnviromentLayerData> layers = new Dictionary<float, EnviromentLayerData>();

        public EnviromentCaravanData enviroment_caravan;

        public float camera_distance = 0;

        public RoadMgr roadMgr = new RoadMgr();

        WorldContext ctx;

        public void init(uint level_id)
        {

            AutoCode.Tables.Level level = new AutoCode.Tables.Level();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "level", out var asset);
            level.load_from(asset);
            level.try_get(level_id, out var t);

            //focus_pos = new Vector2(-dis, 0);

            add_all_resource(t.f_scene_resource);

            foreach (var view in views)
            {
                view.init_enviorment();
            }

            ctx = WorldContext.instance;
            ctx.add_tick(Common.Config.EnviromentMgr_Priority, Common.Config.EnviromentMgr_Name, tick);
            ctx.add_tick_after(Common.Config.EnviromentMgr_Priority, Common.Config.EnviromentMgr_Name, after_tick);
        }

        private void after_tick()
        {
            if (ctx.is_need_reset)
                WorldSceneRoot.instance.mainCamera.transform.position -= new Vector3(ctx.reset_dis, 0, 0);
            var v = focus_pos;
            var config = Common.Config.current;
            var fx = v.x + config.travel_scene_camera_offset_x;
            var fy = v.y;
            var v_parm = ctx.caravan_velocity.magnitude * config.travel_scene_camera_z_k_v;
            var fz = Mathf.Lerp(config.travel_scene_camera_z_max, config.travel_scene_camera_z_min, v_parm);
            var follow = new Vector3(fx, fy, fz);           //跟随点

            var camera_position = WorldSceneRoot.instance.mainCamera.transform.position;
            var cz = config.travel_scene_forced_camera_z ? config.camera_z : camera_position.z + (follow.z - camera_position.z) * config.travel_scene_camera_move_k_z * Common.Config.PHYSICS_TICK_DELTA_TIME;
            var cx = camera_position.x + (follow.x - camera_position.x) * config.travel_scene_camera_move_k_x * Common.Config.PHYSICS_TICK_DELTA_TIME;
            var y_offset = (config.road_z - camera_position.z) * Mathf.Tan(WorldSceneRoot.instance.mainCamera.fieldOfView / 2 * Mathf.Deg2Rad) * (1 - 2 * config.ground_screen_height);
            var cy = follow.y * config.camera_caravan_y_coefficient + y_offset;
            cy = Mathf.Clamp(cy, config.camera_y_min, config.camera_y_max);
            WorldSceneRoot.instance.synchronize_camera(new Vector3(cx, cy, cz));
        }

        private void tick()
        {
            focus_pos = ctx.caravan_pos;
            m_dis = 2 * (Common.Config.current.desiredResolution.x / (float)Common.Config.current.pixelPerUnit) * (1 + Mathf.Abs(WorldSceneRoot.instance.mainCamera.transform.position.z) / 10);
            // 2 * 是因为不能保证车在屏幕中的位置,取极限位置车在最前/最后来处理
            roadMgr.add_road(focus_pos,m_dis);
            roadMgr.remove_road(focus_pos,m_dis); 

            foreach (var p in layers)
            {

                while (need_remove_obj(p))
                {

                }
                while (need_add_obj(p))
                {

                }
            }

            foreach (var view in views)
            {
                view.update_enviroment();
            }

            if (ctx.is_need_reset)
                reset_enviroments();
        }


        public void update_scene_resources(uint[] scene_resoureces_id)
        {

            Debug.Log("update_resources");

            foreach (var layer in layers)
            {
                layer.Value.art_resource_pool.Clear();
            }
            foreach (var id in scene_resoureces_id)
            {
                add_all_resource(id);
            }
        }

        public void move(Vector2 delta)
        {
            if (ctx.caravan_move_status == Common_Formal.Enum.EN_caravan_move_status.idle)
            {
                Caravan_Move_Helper.instance.move(ctx);
            }
                
        }

        public void reset_enviroments()
        {
            var reset_dis = ctx.reset_dis;
            roadMgr.reset_road(reset_dis);
            foreach (var p in layers)
            {

                p.Value.rnd_position_x -= reset_dis;
                p.Value.fixed_distance_x -= reset_dis;

                foreach (var o in p.Value.objSet)
                {
                    o.position.x -= reset_dis;
                }
            }

            foreach (var view in views)
            {
                view.reset_enviroment();
            }
        }

        public bool need_add_obj(KeyValuePair<float, EnviromentLayerData> kp)
        {
            bool add = false;
            var layer = kp.Value;
            if (layer.fixed_distance_x - focus_pos.x < m_dis * kp.Key / 10)
            {        //相机离这一层的生成点太近了!           
                layer.art_resource_pool.TryGetValue("fixed", out var l);
                if (l != null)
                {
                    int index = UnityEngine.Random.Range(0, l.Count);
                    var d = add_random_resource(l[index].Item1, l[index].Item2);
                    float z = kp.Key;
                    foreach (var view in views)
                    {
                        view.add_obj(z, d);
                    }
                    add = true;
                }
                if (layer.fixed_distance_x == 0 && l != null && l.Count != 0)
                {
                    Debug.LogWarning($"{kp.Key} layer 的fixed_distance 为0,请修改");
                    return false;
                }
            }
            if (layer.rnd_position_x - focus_pos.x < m_dis * kp.Key / 10)
            {
                layer.art_resource_pool.TryGetValue("rnd", out var l);
                if (l != null)
                {
                    int index = UnityEngine.Random.Range(0, l.Count);
                    var d = add_random_resource(l[index].Item1, l[index].Item2);
                    float z = kp.Key;

                    foreach (var view in views)
                    {
                        view.add_obj(z, d);
                    }
                    add = true;
                }
                if (layer.rnd_position_x == 0 && l!=null &&l.Count!=0)
                {
                    Debug.LogWarning($"{kp.Key} layer 的rnd_distance_x 为0,请修改");
                    return false;
                }
            }
            return add;
        }
        public bool need_remove_obj(KeyValuePair<float, EnviromentLayerData> kp)
        {
            var layer = kp.Value;

            var remove_objs = new List<EnviromentObjData>();

            foreach (var obj in layer.objSet)
            {
                
                if (focus_pos.x - obj.position.x > m_dis * kp.Key / 10)               //出范围就移除,不考虑是固定还是随机
                {
                    remove_objs.Add(obj);
                }
            }
            if (remove_objs.Count == 0)
            {
                return false;
            }
            foreach (var remove_obj in remove_objs)
            {
                layer.objSet.Remove(remove_obj);
                foreach (var view in views)
                {
                    view.remove_obj(kp.Key, remove_obj);
                }
            }
            return true;
        }

        /// <summary>
        /// 从group_id的池子中取所有资源出来
        /// </summary>
        /// <param name="group_id"></param>
        private void add_all_resource(uint group_id)
        {
            AutoCode.Tables.SceneResource sr = new();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "scene_resource", out var asset);
            sr.load_from(asset);
            foreach (var record in sr.records)
            {
                if (record.f_group_id == group_id)
                {
                    var resource = record;
                    var bundle = resource.f_resource.Item1;
                    var paths = resource.f_resource.Item2;
                    int i = UnityEngine.Random.Range(0, paths.Length);
                    var data = new EnviromentObjData();
                    data.bundle = bundle;
                    data.path = paths[i];

                    if (!layers.ContainsKey(resource.f_z_distance))
                    {           //没有这一层就加上这一层
                        layers.Add(resource.f_z_distance, new EnviromentLayerData
                        {
                            fixed_distance_x = focus_pos.x,
                            rnd_position_x = focus_pos.x,
                            objSet = new(),
                            art_resource_pool = new(),
                        });
                    }
                    update_layer(resource, ref data);
                    if (data != null)
                        layers[resource.f_z_distance].objSet.Add(data);
                }
            }
        }

        /// <summary>
        /// 从编号为group_id的池子里面取sub_id一份资源出来                  10.13   更新随机的物体会根据上一次的active情况判断
        /// </summary>
        private EnviromentObjData add_random_resource(uint group_id, uint sub_id)
        {            //逻辑层 只记录要生成的路径,具体外观生成交给view去做

            AutoCode.Tables.SceneResource sr = new();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "scene_resource", out var asset);
            sr.load_from(asset);
            sr.try_get(group_id, sub_id, out var resource);
            var bundle = resource.f_resource.Item1;
            var paths = resource.f_resource.Item2;
            int i = UnityEngine.Random.Range(0, paths.Length);
            var data = new EnviromentObjData();
            data.bundle = bundle;
            data.path = paths[i];

            if (!layers.ContainsKey(resource.f_z_distance))
            {           //没有这一层就加上这一层
                layers.Add(resource.f_z_distance, new EnviromentLayerData
                {
                    fixed_distance_x = focus_pos.x,
                    rnd_position_x = focus_pos.x,
                    objSet = new(),
                    art_resource_pool = new(),
                });
            }
            update_layer(resource, ref data);
            if (data != null)
                layers[resource.f_z_distance].objSet.Add(data);
            return data;
        }
        /// <summary>
        /// 从这个resource的众多素材里面 随便取一个出来 加入layer 
        /// </summary>
        private bool update_layer(SceneResource.Record resource, ref EnviromentObjData data)
        {
            if (resource.f_rnd_type == SceneResource.e_spawn_type.i_RndPosition)
            {
                float new_x, new_y;
                if (resource.f_rnd_pos == null)
                {
                    Debug.LogWarning("rnd_pos 没有正确的配置");
                    return false;
                }
                var pos = resource.f_rnd_pos;

                new_x = layers[resource.f_z_distance].rnd_position_x + UnityEngine.Random.Range(pos.f_x.Item1, pos.f_x.Item2) * resource.f_z_distance / 10;
                new_y = UnityEngine.Random.Range(pos.f_y.Item1, pos.f_y.Item2) * resource.f_z_distance / 10;

                data.position = new Vector2(layers[resource.f_z_distance].rnd_position_x, layers[resource.f_z_distance].rnd_position_y);
                data.obj_spwan_type = SceneResource.e_spawn_type.i_RndPosition;

                layers[resource.f_z_distance].rnd_position_x = new_x;
                layers[resource.f_z_distance].rnd_position_y = new_y;

                if (layers[resource.f_z_distance].art_resource_pool.TryGetValue("rnd", out var lists))
                {
                    lists.Add((resource.f_group_id,resource.f_sub_id));
                }
                else
                {
                    List<(uint,uint)> temp = new List<(uint, uint)>();
                    temp.Add((resource.f_group_id, resource.f_sub_id));
                    layers[resource.f_z_distance].art_resource_pool.Add("rnd", temp);
                }

                if (resource.f_rnd_p!=null&&resource.f_rnd_p.f_active != 0 && resource.f_rnd_p.f_muted != 0)
                {
                    float r = UnityEngine.Random.value;
                    
                    if (layers[resource.f_z_distance].obj_active.TryGetValue((resource.f_group_id, resource.f_sub_id), out bool b))             //判断这类是否不是第一次刷新
                    {
                        if (b)
                        {
                            if (r < resource.f_rnd_p.f_active)
                            {

                            }
                            else
                            {
                                data = null;
                                layers[resource.f_z_distance].obj_active[(resource.f_group_id, resource.f_sub_id)] = false;
                            }
                        }
                        else
                        {
                            if (r < resource.f_rnd_p.f_muted)
                            {
                                data = null;
                            }
                            else
                            {
                                layers[resource.f_z_distance].obj_active[(resource.f_group_id, resource.f_sub_id)] = true;
                            }
                        }
                    }
                    else                                                //是第一次刷新
                    {
                        if(r < resource.f_rnd_p.f_muted)
                        {
                            data = null;
                            layers[resource.f_z_distance].obj_active.Add((resource.f_group_id, resource.f_sub_id), false);
                        }
                        else
                        {
                            layers[resource.f_z_distance].obj_active.Add((resource.f_group_id, resource.f_sub_id), true);
                        }
                    }
                }
            }
            else if (resource.f_rnd_type == SceneResource.e_spawn_type.i_FixedDistance)
            {
                float new_x, new_y = 0f;

                new_x = layers[resource.f_z_distance].fixed_distance_x;

                data.position = new Vector2(new_x, new_y);                  //之后外观的位置会是 prefab的位置 + position
                data.obj_spwan_type = SceneResource.e_spawn_type.i_FixedDistance;

                layers[resource.f_z_distance].fixed_distance_x = new_x + resource.f_fixed_length * resource.f_z_distance / 10;

                if (layers[resource.f_z_distance].art_resource_pool.TryGetValue("fixed", out var lists))
                {
                    lists.Add((resource.f_group_id, resource.f_sub_id));
                }
                else
                {
                    List<(uint, uint)> temp = new List<(uint, uint)>();
                    temp.Add((resource.f_group_id, resource.f_sub_id));
                    layers[resource.f_z_distance].art_resource_pool.Add("fixed", temp);
                }
            }
            return true;
        }

    }
    public class EnviromentCaravanData
    {
        public Vector2 rotation;        //目前是用不上的
        public Vector2 position;
        public CaravanMgr_Formal caravan;
        public bool is_run = false;

        public Vector2 init_offset;
    }


    public class EnviromentObjData
    {

        public string bundle;
        public string path;
        public SceneResource.e_spawn_type obj_spwan_type;

        public Vector2 position;
        public float scale;
    }

    public class EnviromentLayerData
    {
        public Dictionary<(uint, uint), bool> obj_active  = new();           //某一类的物体上次刷新是否是激活的

        public Dictionary<string, List<(uint,uint)>> art_resource_pool = new();

        public HashSet<EnviromentObjData> objSet = new();

        public float fixed_distance_x;          //下一次生成的固定背景的x

        public float rnd_position_x;            //下一次生成的随机物体的x

        public float rnd_position_y;            //下一次生成的随机物体的y
    }
}
