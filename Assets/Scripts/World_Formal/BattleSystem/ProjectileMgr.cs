using System.Collections.Generic;
using Common_Formal;
using Foundation;
using World_Formal.BattleSystem.Device;

namespace World_Formal.BattleSystem
{
    public class ProjectileMgr :IMgr
    {
        string IMgr.name => m_mgr_name;
        readonly string m_mgr_name;

        public ProjectileMgr(string name,params object[] objs)
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

        //====================================================================

        public List<WorldProjectile> projectiles = new();

        public void AddProjectile(Projectile p)
        {
            EX_Utility.try_load_asset(p.desc.f_prefab.Item1, p.desc.f_prefab.Item2, out WorldProjectileView pv);
            var pv_entity = UnityEngine.GameObject.Instantiate(pv,WorldSceneRoot.instance.projectiles_root,false);
            var wp = new WorldProjectile(p);
            wp.add_view(pv_entity);
            projectiles.Add(wp);
        }
        public void tick()
        {
            foreach(var p in projectiles)
            {
                p.tick();
            }

            for(int i = 0; i < projectiles.Count; i++)
            {
                if(projectiles[i].validate == false)
                {
                    projectiles[i].clear_views();
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void DestroyProjectile(Projectile p)
        {
            for(int i = 0; i < projectiles.Count; i++)
            {
                if (projectiles[i].projectile == p)
                {
                    projectiles[i].validate = false;
                }
            }
        }
    }
}
