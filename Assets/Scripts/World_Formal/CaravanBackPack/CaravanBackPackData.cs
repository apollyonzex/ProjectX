using System.Collections.Generic;
using UnityEngine;
using World_Formal.Caravans.Devices;

namespace World_Formal.CaravanBackPack
{
    /// <summary>
    /// 蓬车背包的信息
    /// </summary>
    [System.Serializable]
    public class CaravanBackPackData : ScriptableObject {

        public List<CaravanBackPackCellData> cells = new List<CaravanBackPackCellData>();

        public List<CaravanBackPackAreaData> areas = new List<CaravanBackPackAreaData>();

        public List<CaravanBackPackItemData> items = new List<CaravanBackPackItemData>();
    }
    /// <summary>
    /// 篷车的背包的格子的信息
    /// </summary>
    [System.Serializable]
    public class CaravanBackPackCellData {
        public bool occupied;

        public bool locked;

        public Vector2Int position;

        public CaravanBackPackCellData(bool occupied, bool locked,(int,int) position) {
            this.occupied = occupied;
            this.locked = locked;
            this.position = new Vector2Int(position.Item1,position.Item2);
        }
    }
    /// <summary>
    /// 篷车背包内存的设备的信息
    /// </summary>
    [System.Serializable]
    public class CaravanBackPackItemData {
        public Device device;
        public Vector2Int position;
        public bool rotatoed;
        public Vector2Int scale;

        public CaravanBackPackItemData(Device device, (int, int) position, bool rotatoed,(int,int) scale) {
            this.device = device;
            this.position = new Vector2Int(position.Item1, position.Item2);
            this.rotatoed = rotatoed;
            this.scale = new Vector2Int(scale.Item1, scale.Item2);
        }
    }
    /// <summary>
    ///  篷车背包内的区域信息
    /// </summary>
    [System.Serializable]
    public class CaravanBackPackAreaData {

        
        private bool locked;
        [SerializeField]
        public bool Locked {
            get {
                return locked;
            }
            set {
                locked = value;
                foreach(var cell in cellList) {
                    cell.locked = value;
                }
            }
        }

        public List<CaravanBackPackCellData> cellList = new List<CaravanBackPackCellData>();
    }
}


