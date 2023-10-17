using Camp;
using Camp.Level_Entrances;
using Common_Formal;
using UnityEngine;
using UnityEngine.UI;

public class Win_Level_Entrance : MonoBehaviour
{
    public Level_Entrance_Asset data;
    public Transform level_entrances_node;

    CampRootContext ctx;
    CampSceneRoot root;

    //==================================================================================================

    public void init()
    {
        ctx = CampRootContext.instance;
        root = CampSceneRoot.instance;

        Level_EntranceMgr mgr;
        if (Mission.instance.try_get_mgr("Level_Entrance", out var imgr))
        {
            mgr = (Level_EntranceMgr)imgr;
        }
        else
        {
            mgr = new("Level_Entrance");
        }

        foreach (var e in data.cells)
        {
            Camp.DB.instance.world.try_get(e.world_id, out var r);

            Level_Entrance cell = new(e);
            cell.level_id = r.f_level_list[cell.seq];

            EX_Utility.create_cell_in_scene<Level_EntranceMgr, ILevel_EntranceView, Level_EntranceView>(mgr, r.f_entrance_view, level_entrances_node, out var view);
            view.gameObject.name = e.name;
            view.name.text = e.name;
            view.transform.localPosition = e.pos;
            mgr.add_cell(mgr.get_key(cell), cell, view);

            var btn = view.gameObject.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                @do(e.world_id,cell.level_id);
            });
        }
    }


    void @do(uint id)
    {
        Debug.Log($"已选择关卡：{id}");
        ctx.flow = CampRootContext.EN_Flow_State.select_level_completed;
        ctx.level_id = id;

        gameObject.SetActive(false); //关闭窗口
        root.open_win_select_carbody();
    }

    void @do(uint world_id,uint level_id) {
        Debug.Log($"已选择世界：{world_id}  关卡: {level_id}");
        ctx.world_id = world_id;
        ctx.level_id = level_id;
        ctx.flow = CampRootContext.EN_Flow_State.select_level_completed;

        gameObject.SetActive(false); //关闭窗口
        root.open_win_select_carbody();
    }
}
