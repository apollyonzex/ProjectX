using Editor.DIY_Editor;
using Camp.Level_Entrances;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using Camp;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level_Entrance_Editor
{
    public class Level_Entrance_Root : Root<Level_Entrance_Asset>
    {
        public Transform ui_root;

        Object[] cells => FindObjectsOfType(typeof(CellData));

        internal CellBrush m_brush;

        //==================================================================================================

        public override void load_asset()
        {
            clear();

            foreach (var cell in asset.cells)
            {
                var world_id = cell.world_id;
                DB.instance.world_without_running.try_get(world_id, out var r);
                m_brush.create_cell(r, world_id, cell.pos, cell.name, cell.size, cell.seq);
            }
        }


        protected override void save_asset(Level_Entrance_Asset asset)
        {
            List<Level_Enterance_Serializable> save_cells = new();

            foreach (var cell in cells)
            {
                var data = (CellData)cell;
                var lp = data.transform.localPosition;
                var rect = data.GetComponent<RectTransform>();

                Level_Enterance_Serializable save_cell = new()
                {
                    name = data.name,
                    seq = data.seq,
                    pos = lp,
                    size = rect.sizeDelta,
                    world_id = data.world_id
                };

                save_cells.Add(save_cell);
            }

            asset.cells = save_cells.ToArray();          
        }


        public void @clear()
        {
            foreach (var cell in cells)
            {
                var e = (CellData)cell;
                DestroyImmediate(e.gameObject);
            }
        }
    }
}

