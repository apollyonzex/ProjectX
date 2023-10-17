
using Common;
using System.Collections.Generic;

namespace ProcessEditor.Enemys
{
    public class EnemyMgr : Singleton<EnemyMgr>
    {
        public Dictionary<uint, AutoCode.Tables.EnemyNormal.Record> raw = new();
        public uint default_id = 1;

        //==================================================================================================

        public EnemyMgr()
        {
            Common.Expand.Utility.try_load_table_without_running("enemy", "enemy_normal", out AutoCode.Tables.EnemyNormal land);
            foreach (var e in land.records)
            {
                raw.Add(e.f_id, e);
            }
        }
    }
}

