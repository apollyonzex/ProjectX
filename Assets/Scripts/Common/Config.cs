using UnityEngine;


namespace Common {

    [CreateAssetMenu(menuName = "GameConfig", fileName = "GameConfig")]
    public class Config : ScriptableObject {

        #region codes
        public static Config current {
            get {
                if (s_current == null) {
                    s_current = CreateInstance<Config>();
                }
                return s_current;
            }
        }
        private static Config s_current;
        private void OnEnable() {
            s_current = this;
        }
        private void OnDisable() {
            if (ReferenceEquals(s_current, this)) {
                s_current = null;
            }
        }
        #endregion

        [Range(1, 256)]
        public int pixelPerUnit = 100;

        public float scaled_pixel_per_unit { get; set; } = 100;

        public Vector2Int desiredResolution = new Vector2Int(1920, 1080);

        public float desiredPerspectiveFOV = 60;

        [Header("营地相关")]
        public float basement_camera_move_speed =  0.5f;


        [Header("卡牌相关")]

        public int card_num_init = 2;
        public int card_num_max = 5;
        public int card_draw_interval = 180;
        public int soul_increase_rate = 833;

        [Header("车体运动")]
        public float driving_speed_limit_min = 2f;
        public float driving_acc_min = 0.3f;
        public float gravity = -9.8f;
        public int t_floating = 18;
        public float caravan_rotate_speed = 5f;
        public float caravan_rotate_angle_limit = 60f;

        [Header("临时测试")]
        public int bullet_damage = 50;
        public Vector3 battle_camera_localPosition;
        public float reset_pos_intervel = 25.6f;
        public Vector2 show_enemy_area = new Vector2(25.6f, 20);//在范围内, 敌人显示，否则以数据形式保存
        public bool open_damage_to_enemy = true;//是否开启伤害
        public Color be_damaged_color = new Color(241,95,95,255);//受击颜色
        public float be_damaged_recover_time = 0.5f;//受击后x秒后恢复颜色
        public float enemy_dead_time = 3f;//怪物hp归0后，x秒后死亡
        public double win_distance = 2500f; //篷车达到x时获胜

        [Header("镜头")]
        public float move_weight_when_caravan_acc = 0.1f;//篷车速度变化时，镜头的移动系数
        public float move_dis_limit = 10f;//篷车速度变化时，镜头的移动距离上限

        public Vector3 focus_offset = new(0,4);

        [Header("敌人行为")]
        public int passenger_num_max = 5;//最大乘客数量，顶住大篷车时使用
        public float passenger_queue_padding = 0.2f;
        public float passenger_queue_spacing = 0.2f;

        public int passenger_driving_speed_limit_modify = 2000;
        public int passenger_driving_acc_modify = 800;

        public float enemy_gravity = -30f;
        public int enemy_landing_ticks = 20;

        public int boarder_driving_speed_limit_modify = 1000;
        public int boarder_driving_acc_modify = 400;

        public int boarding_atk_base = 1200;
        public int boarding_dot = 1;//扒车dot固伤

        [Header("敌人运动")]
        public float _impact_a = 1f;//击退速度的自然消减速度


        [Header("程序调试用")]
        public bool formal = false;

        [Header("场景")]
        public float default_speed;
        public Sprite map_caravan;
        public float map_speed ;
        public float road_altitude__init;
        public Vector2 road_altitude_limit;

        [Header("冒险等级")]
        public int max_player_level;
		public int max_device_level;

        [Header("镜头运动")]
        public float camera_z;
        public float road_z;
        public float ground_screen_height;
        public float camera_caravan_y_coefficient;
        public float camera_y_max;
        public float camera_y_min;
        public float travel_scene_camera_move_k_x;
        public float travel_scene_camera_offset_x;
        public float travel_scene_camera_move_k_z;
        public float travel_scene_camera_z_min;
        public float travel_scene_camera_z_max;
        public float travel_scene_camera_z_k_v;
        public bool travel_scene_forced_camera_z;

        [Header("旅行场景参数")]
        public float anim_idle_when_below_velocity = 0.1f;

        [Header("気 && 卡牌")]
        public int qi_max;
        public int qi_rising_rate;

        [Header("新敌人")]
        public (string,string) default_bt_graph_path = ("monster_behaviour_tree", "mbt_default");
        public float impact_a = 40f;
        public float between_holding_monster_offset = 0.5f; //顶车状态下的怪物间距
        
        #region const
        //帧率
        public const int PHYSICS_TICKS_PER_SECOND = 120;
        public const float PHYSICS_TICK_DELTA_TIME = 1f / PHYSICS_TICKS_PER_SECOND;
        #endregion

        #region internal_setting
        //Mgr tick优先级
        public const int CaravanMgr_Priority = 1;
        public const int DeviceMgr_Priority = 2;
        public const int EnemyMgr_Priority = 3;
        public const int EnviromentMgr_Priority = 4;
        public const int MapMgr_Priority = 5;
        public const int CardMgr_Priority = 6;
        public const int ProjectileMgr_Priority = 7;
        public const int ProjectileMgr_Enemy_Priority = 8;

        //Mgr注册name(tick)
        public const string ProjectileMgr_Name = "ProjectileMgr";
        public const string GarageMgr_Name = "GarageMgr";
        public const string CaravanMgr_Name = "CaravanMgr";
        public const string DeviceMgr_Name = "DeviceMgr";
        public const string EnemyMgr_Name = "EnemyMgr";
        public const string EnviromentMgr_Name = "enviroment";
        public const string CardMgr_Name = "CardMgr";
        public const string WareHouseMgr_Name = "WareHouse";
        public const string MapMgr_Name = "MapMgr";
        public const string ProjectileMgr_Enemy_Name = "ProjectileMgr_Enemy";

        //Mgr注册name(普通)
        public const string RewardHouse_Name = "RewardHouse";

        #endregion

#if UNITY_EDITOR
        private void OnValidate() {
           
        }
#endif
    }

}