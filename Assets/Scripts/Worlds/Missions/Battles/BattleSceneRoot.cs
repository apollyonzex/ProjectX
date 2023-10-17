using Common;
using Devices;
using DeviceViews;
using Foundation;
using UnityEngine;
using Worlds.Missions.Battles.Caravan;
using Worlds.Missions.Battles.Enemies;

namespace Worlds.Missions.Battles
{
    public class BattleSceneRoot : SceneRoot<BattleSceneRoot>
    {
        public Transform caravan;
        public Transform devices;
        public Transform enemies;
        public Transform projectiles;
        public CameraShot.Focus focus;
        public Transform effects;

        public float battlefield_length;

        //==================================================================================================

        public const int PHYSICS_TICKS_PER_SECOND = 120;
        public const float PHYSICS_TICK_DELTA_TIME = 1f / PHYSICS_TICKS_PER_SECOND;

        bool m_fighting = false;
        float m_physics_tick_timer = 0f;

        public static event System.Action physics_tick;
        public static event System.Action physics_tick_after;

        public BattleMgr battleMgr => m_battleMgr;
        BattleMgr m_battleMgr;

        public BattleCardMgr battlecardMgr => m_battlecardMgr;
        BattleCardMgr m_battlecardMgr = new BattleCardMgr();


        WorldSceneRoot m_root;

        //==================================================================================================

        public void start_battle(BattleMgr mgr)
        {
            m_battleMgr = mgr;
            mgr.get_battlefield_info(out battlefield_length);

            physics_tick += mgr.physics_tick;
            m_fighting = true;

            mainCamera.transform.SetParent(focus.transform);
            mainCamera.transform.localPosition = Vector3.zero;
        }


        public void end_battle()
        {
            m_fighting = false;
            physics_tick -= m_battleMgr.physics_tick;

            mainCamera.transform.SetParent(m_root.focus);
            mainCamera.transform.localPosition = Vector3.zero;
        }


        private void Start()
        {
            Physics2D.autoSyncTransforms = false;
            Physics2D.simulationMode = SimulationMode2D.Script;

            m_root = WorldSceneRoot.instance;
            var cardview = m_root.Open_UI_Prefab<BattleCardMgrView>("card", "BattleCardPanel", m_root.mission_layer.transform);
            m_battlecardMgr.add_view(cardview);
            m_battlecardMgr.Duel();

            mainCamera = m_root.mainCamera;
            uiCamera = m_root.uiCamera;
            uiRoot = m_root.uiRoot;
        }


        void Update()
        {
            if (!m_fighting) return;

            m_physics_tick_timer += Mathf.Min(Time.deltaTime, PHYSICS_TICK_DELTA_TIME * Time.timeScale);
            while (m_physics_tick_timer >= PHYSICS_TICK_DELTA_TIME)
            {
                m_physics_tick_timer -= PHYSICS_TICK_DELTA_TIME;

                physics_tick?.Invoke();
                Physics2D.SyncTransforms();
                Physics2D.Simulate(PHYSICS_TICK_DELTA_TIME);
                physics_tick_after?.Invoke();
            }
        }


        public void create_caravan_view(Caravan.BattleCaravanMgr mgr, Caravan.BattleCaravan cell, string bundle, string path, out IBattleCaravanView iview)
        {
            var prefab = Common.Utility.load_asset<Caravan.BattleCaravanView>(bundle, path);
            var view = Instantiate(prefab, caravan);
            view.init(cell);
            mgr.add_view(view);

            var collider = view.box_collider;
            cell.collider_offset = collider.offset;
            cell.collider = collider.size;

            cell.sm = view.sm;
            cell.anm = view.anm;
            cell.sm_pos_offset = view.sm.transform.localPosition;

            iview = view;
        }


        public void create_device_view(Devices.BattleDeviceMgr mgr, Devices.BattleDevice cell, string bundle, string path, out Devices.BattleDeviceView view)
        {
            var prefab = Common.Utility.load_asset<GameObject>(bundle, path);
            var g = Instantiate(prefab, devices);

            view = g.AddComponent<Devices.BattleDeviceView>();
            view.init(cell);
            mgr.add_view(view);
            mgr.devices.Add(cell, view);
        }


        public void create_enemy_view(Enemies.EnemyMgr mgr, Enemies.Enemy cell, string bundle, string path, out Enemies.EnemyView view)
        {
            var prefab = Common.Utility.load_asset<EnemyView>(bundle, path);
            view = Instantiate(prefab, enemies);
            view.init(cell);

            var g = view.gameObject;
            g.name = cell._id.ToString();

            Physics2D.SyncTransforms();         //待讨论/设计
            mgr.add_view(view);

            var dic = mgr.enemies;
            if (dic.ContainsKey(cell))
                dic[cell] = view;
            else
                dic.Add(cell, view);

            var collider = g.GetComponent<BoxCollider2D>();
            cell.collider_offset = collider.offset;
            cell.collider = collider.size;
        }


        public void create_projectile_view(Projectiles.ProjectileMgr mgr, Projectile cell, string bundle, string path, out ProjectileView view)
        {
            var prefab = Common.Utility.load_asset<GameObject>(bundle, path);
            var g = Instantiate(prefab, projectiles);
            if (g.TryGetComponent<ProjectileView>(out var component))
            {
                view = component;
            }
            else
            {
                view = g.AddComponent<ProjectileView>();
            }
            view.init(cell);
            mgr.add_view(view);
            mgr.projectiles.Add(cell, view);
        }


        public void create_effect_view(Effects.BattleEffectMgr mgr, string bundle, string path, float lifetime, Vector2 position)
        {
            var prefab = Common.Utility.load_asset<GameObject>(bundle, path);
            var g = Instantiate(prefab, effects);

            var view = g.AddComponent<Effects.BattleEffectView>();
            view.init(position);
            mgr.add_view(view);

            Destroy(g, lifetime);
            g.transform.localPosition = position;
        }

        //拓展：
        GameObject m_boom;
        public void delay_boom(GameObject obj, float time)
        {
            m_boom = obj;
            Invoke(nameof(do_boom), time);
        }


        public void do_boom()
        {
            var caravan = battleMgr.caravan_mgr.caravan;
            caravan.driving_speed_limit = 7500;
            caravan.velocity.x = 7.5f;
            caravan.velocity.y = 0;
            DestroyImmediate(m_boom);
        }


        System.Action m_add_enemy_action;
        /// <summary>
        /// 随机延迟，添加怪物
        /// </summary>
        public void add_enemy_per_random_time(System.Action action)
        {
            m_add_enemy_action = action;
            Invoke(nameof(add_enemy), Random.Range(0, 1.5f));
        }

        void add_enemy()
        {
            m_add_enemy_action?.Invoke();
        }



    }

}

