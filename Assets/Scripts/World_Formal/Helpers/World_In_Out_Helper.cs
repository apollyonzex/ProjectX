using Common_Formal;
using World_Formal.Adventure;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans;
using World_Formal.Map.Journey;

namespace World_Formal.Helpers
{
    public class World_In_Out_Helper : Singleton<World_In_Out_Helper>
    {
        public void enter_world(uint world_id, uint level_id)
        {
            if (Mission.instance.try_get_mgr("adventure", out AdventureMgr amgr))
                amgr.clear_views();

            if (Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gmgr))
                gmgr.clear_views();

            new JourneyMgr("journey", world_id, level_id);
            Road_Info_Helper.reset();

            EX_Utility.load_game_state("World_Formal.WorldState", "World_Formal");
        }


        public void leave_world()
        {
            Mission.instance.try_get_mgr(Common.Config.CaravanMgr_Name, out CaravanMgr_Formal cvmgr);
            Mission.instance.try_get_mgr(Common.Config.GarageMgr_Name, out GarageManager gmgr);
            gmgr.RemoveCaravan(cvmgr);
            gmgr.AddCaravan(cvmgr);

            EX_Utility.load_game_state("Camp.CampState", "Camp");
        }
    }
}

