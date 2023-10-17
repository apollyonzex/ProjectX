using System.Collections.Generic;
using Common_Formal;
using World_Formal.CaravanBackPack;

public class CampRootContext : Singleton<CampRootContext>
{
    public uint world_id;
    public uint level_id;
    public EN_Flow_State flow = EN_Flow_State.init;

    public List<Slot_Info_View> views = new();

    //==================================================================================================

    /// <summary>
    /// 流程状态：玩家已完成的步骤
    /// </summary>
    public enum EN_Flow_State
    {
        init,
        select_level_completed,
        select_carbody_completed
    }
}
