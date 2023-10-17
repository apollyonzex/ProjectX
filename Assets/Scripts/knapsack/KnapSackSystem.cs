using Foundation;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World_Formal.CaravanBackPack;
using World_Formal.Caravans.Devices;

namespace BackPack
{
    [ExecuteInEditMode]
    public class KnapSackSystem : MonoBehaviourSingleton<KnapSackSystem> {

        #region viewPart
        public KnapSackCell cellPrefab;
        public Transform content;

        public void start() {
            init();
        }

        #endregion
        #region  save
        public CaravanBackPackData data;

        public void save() {
            if (data != null) {

            } else {
                var  t = ScriptableObject.CreateInstance<CaravanBackPackData>();
                AssetDatabase.CreateAsset(t, @"Assets\Scripts\knapsack\Data\" + "new backpackdata" + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                data = t;
            }

            data.cells.Clear();
            data.areas.Clear();
            data.items.Clear();

            for(int i = 0; i < row; i++) {
                for(int j = 0; j < col; j++) {
                    var cell = backpack[i, j];
                    data.cells.Add(new CaravanBackPackCellData(cell.occupied,cell.locked,(cell.row,cell.col)));
                }
            }

            foreach(var area in areaList) {
                CaravanBackPackAreaData areaData = new CaravanBackPackAreaData();
                areaData.Locked = area.locked;
                foreach(var cell in area.cells) {
                    areaData.cellList.Add(data.cells[cell.row * 14 + cell.col]);
                }
                data.areas.Add(areaData);
            }

            foreach(var device in deviceList) {
                CaravanBackPackItemData itemData = new(new Device((uint)device.index),device.position,device.rotatoed,device.size);
                data.items.Add(itemData);
            }
            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssetIfDirty(data);
            Debug.Log("save");
        }

        #endregion
        public int row = 9 , col  = 14;

        public int item_id;

        public KnapSackCell[,] backpack = new KnapSackCell[9, 14];  
        public List<KnapSackArea> areaList = new List<KnapSackArea>();
        public List<TempDevice> deviceList = new List<TempDevice>();

        public void init() {
            for (int i = 0; i < row; i++) {
                for (int j = 0; j < col; j++) {
                    var cell =Instantiate(cellPrefab, content, false);
                    cell.gameObject.SetActive(true);
                    cell.gameObject.name = $"({i},{j}) cell";
                    cell.row = i;
                    cell.col = j;

                    backpack[i, j] = cell;
                }
            }
        }
        /// <summary>
        /// 尝试放置设备
        /// </summary>
        public bool try_input_device(int _row, int _col, TempDevice device) {
            if (backpack == null) {
                Debug.LogWarning("backpack is null  ,but try to input device");
                return false;
            }
            if (!put_device(_row, _col, device)) {
                rotato_device(ref device);
                if (!put_device(_row, _col, device)) {
                    return false;
                }
            }
            device.position = (_row, _col);
            deviceList.Add(device);
            return true;
        }
        /// <summary>
        /// 尝试移除设备
        /// </summary>
        public bool try_remove_device(int _row,int _col) {
            if (backpack == null) {
                Debug.LogWarning("backpack is null  ,but try to remove device");
                return false;
            }
            TempDevice d = new TempDevice();
            foreach (var device in deviceList) {
                if(device.position == (_row, _col)) {
                    d = device;
                    var pos_x = device.position.Item1;
                    var pos_y = device.position.Item2;
                    var hori = device.size.Item1;
                    var vert = device.size.Item2;
                    if (device.rotatoed) {
                        var temp = hori;
                        hori = vert;
                        vert = temp;
                    }
                    for (int i = pos_x; i < pos_x + hori; i++) {
                        for (int j = pos_y; j < pos_y + vert; j++) {
                            backpack[i, j].occupied = false;
                        }
                    }
                    break;
                }
            }

            deviceList.Remove(d);

            return true;
        }

        /// <summary>
        /// 判断设备能否放置
        /// </summary>
        private bool put_device(int _row, int _col, TempDevice device) {
            var hori = device.size.Item1;
            var vert = device.size.Item2;
            if(device.rotatoed == true) {
                var temp = hori;
                hori = vert;
                vert = temp;
            }

            if ((_row + hori) > row) {
                return false;
            }
            if ((_col + vert) > col) {
                return false;
            }
            for (int i = _row; i < (_row + hori); i++) {
                for (int j = _col; j < (_col + vert); j++) {
                    if (!backpack[i,j].CanUse()) {
                        return false;
                    }
                }
            }
            for (int i = _row; i < (_row + hori); i++) {
                for (int j = _col; j < (_col + vert); j++) {
                    backpack[i,j].occupied = true;
                }
            }
            return true;
        }

        private void rotato_device(ref TempDevice device) {
            device.rotatoed = !device.rotatoed;
        }



        private void Update() {
            SynArea();
            SynDevice();
        }


        private void SynArea() {
            foreach (var area in areaList) {
                foreach (var cell in area.cells) {
                    cell.locked = area.locked;
                }
            }
        }

        private void SynDevice() {
            foreach(var device in deviceList) {
                var pos_x = device.position.Item1;
                var pos_y = device.position.Item2;
                var hori = device.size.Item1;
                var vert = device.size.Item2;
                if(device.rotatoed) {
                    var temp = hori;
                    hori = vert;
                    vert = temp;
                }


                for(int i = pos_x; i < pos_x + hori; i++) {
                    for(int j = pos_y; j < pos_y + vert; j++) {
                        backpack[i,j].item_color =  device.color;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class TempDevice {
        public Color color = new Color(205,92,92,255);
        public bool rotatoed = false;
        public int index;
        public (int, int) size;
        public (int, int) position;
    }

}

