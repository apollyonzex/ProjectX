

using Foundation;
using Foundation.Tables;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.Enviroment.Road
{

    public interface IRoadMgrView : IModelView<RoadMgr>
    {
        void init();

        void add_curve(Curve curve);

        void remove_curve(Curve curve);

        void reset_curve(float delta);
    }
    public class RoadMgr : Model<RoadMgr, IRoadMgrView>
    {

        public EnviromentMgr owner;

        public Vector2 end_pos;

        public List<AutoCode.Tables.SceneRoad.Record> road_records = new();

        public List<Curve> curves = new();

        public void init(uint group_id,EnviromentMgr owner)
        {
            this.owner = owner;
            AutoCode.Tables.SceneRoad sr = new();
            AssetBundleManager.instance.load_asset<BinaryAsset>("db", "scene_road", out var asset);
            sr.load_from(asset);
            foreach (var record in sr.records)
            {
                if (record.f_group_id == group_id)
                {
                    road_records.Add(record);
                }
            }
            var rc = get_random_record();
            AssetBundleManager.instance.load_asset<RoadData>(rc.f_road_data.Item1, rc.f_road_data.Item2, out var road_data);
            if (road_data == null)
            {
                Debug.LogError("获取road数据失败");
            }

            end_pos = owner.focus_pos - new Vector2(owner.dis,0);
            while (end_pos.x < 0)
            {
                add_curve();
            }


            foreach (var view in views)
            {
                view.init();
            }
        }

        public void add_road(Vector2 focus_pos)
        {
            if ((end_pos.x - focus_pos.x) <= Common.Config.current.desiredResolution.x / Common.Config.current.pixelPerUnit)
            {
                add_curve();
            }
        }

        public void add_road(Vector2 focus_pos,float dis)
        {
            if ((end_pos.x - focus_pos.x) <= dis)
            {
                add_curve();
            }
        }

        public void remove_road(Vector2 focus_pos)
        {
            List<Curve> remove_curves = new();
            foreach (var curve in curves)
            {
                if (focus_pos.x - (curve.start_pos.x + curve.points[curve.points.Count - 1].position.x) > Common.Config.current.desiredResolution.x / Common.Config.current.pixelPerUnit)
                {
                    remove_curves.Add(curve);
                }
            }
            foreach (var curve in remove_curves)
            {
                curves.Remove(curve);

                foreach (var view in views)
                {
                    view.remove_curve(curve);
                }
            }

            remove_curves.Clear();
        }
        public void remove_road(Vector2 focus_pos,float dis)
        {
            List<Curve> remove_curves = new();
            foreach (var curve in curves)
            {
                if (focus_pos.x - (curve.start_pos.x + curve.points[curve.points.Count - 1].position.x) > dis)
                {
                    remove_curves.Add(curve);
                }
            }
            foreach (var curve in remove_curves)
            {
                curves.Remove(curve);

                foreach (var view in views)
                {
                    view.remove_curve(curve);
                }
            }

            remove_curves.Clear();
        }

        public void reset_road(float delta)
        {
            end_pos.x -= delta;
            foreach (var curve in curves)
            {
                curve.start_pos.x -= delta;
            }

            foreach (var view in views)
            {
                view.reset_curve(delta);
            }
        }

        /// <summary>
        /// 输入当前的x,输出这个x的y
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float road_height(float x)
        {
            foreach (var curve in curves)
            {
                if (x >= curve.start_pos.x && x <= (curve.start_pos.x + curve.points[curve.points.Count - 1].position.x))
                {
                    for (int i = 0; i < curve.points.Count - 1; i++)
                    {
                        var p1 = curve.points[i].position + curve.start_pos;
                        var p2 = curve.points[i].right_position + curve.start_pos;
                        var p3 = curve.points[i + 1].left_position + curve.start_pos;
                        var p4 = curve.points[i + 1].position + curve.start_pos;
                        if (x >= p1.x && x <= p4.x)
                        {
                            var t = (x - p1.x) / (p4.x - p1.x);
                            var y = Mathf.Pow((1 - t), 3) * p1.y + 3 * Mathf.Pow((1 - t), 2) * t * p2.y + 3 * (1 - t) * t * t * p3.y + Mathf.Pow(t, 3) * p4.y;
                            return y;
                        }
                    }
                }
            }
            //Debug.Log($"无效的x输入:{x},返回默认y值 0");
            return 0;
        }
        /// <summary>
        /// 返回x位置路段的斜率
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float road_slope(float x)
        {
            foreach (var curve in curves)
            {
                if (x >= curve.start_pos.x && x <= (curve.start_pos.x + curve.points[curve.points.Count - 1].position.x))
                {
                    for (int i = 0; i < curve.points.Count - 1; i++)
                    {
                        var p1 = curve.points[i].position + curve.start_pos;
                        var p2 = curve.points[i].right_position + curve.start_pos;
                        var p3 = curve.points[i + 1].left_position + curve.start_pos;
                        var p4 = curve.points[i + 1].position + curve.start_pos;
                        if (x >= p1.x && x <= p4.x)
                        {
                            var t = (x - p1.x) / (p4.x - p1.x);
                            var s = 1 - t;
                            var y1 = -3 * Mathf.Pow(s, 2) * p1.y + 3 * s * (1 - 3 * t) * p2.y + 3 * (2 - 3 * t) * t * p3.y + 3 * t * t * p4.y;
                            var result = y1 / (p4.x - p1.x);
                            return result;
                        }
                    }
                }
            }


            //Debug.Log($"无效的x输入:{x},返回默认y'值 0");
            return 0;
        }
        /// <summary>
        /// 返回路段x位置的 凹凸性,>0下凹,<0 凸,=0 平
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float road_bump(float x)
        {
            foreach (var curve in curves)
            {
                if (x >= curve.start_pos.x && x <= (curve.start_pos.x + curve.points[curve.points.Count - 1].position.x))
                {
                    for (int i = 0; i < curve.points.Count - 1; i++)
                    {
                        var p1 = curve.points[i].position + curve.start_pos;
                        var p2 = curve.points[i].right_position + curve.start_pos;
                        var p3 = curve.points[i + 1].left_position + curve.start_pos;
                        var p4 = curve.points[i + 1].position + curve.start_pos;
                        if (x >= p1.x && x <= p4.x)
                        {
                            var t = (x - p1.x) / (p4.x - p1.x);
                            var s = 1 - t;
                            var y2 = 6 * s * p1.y + 6 * (3 * t - 2) * p2.y + 6 * (1 - 3 * t) * p3.y + 6 * t * p4.y;
                            var d = (p4.x - p1.x);
                            var result = y2 / (Mathf.Pow(d, 2));
                            return result;
                        }
                    }
                }
            }


            Debug.Log($"无效的x输入:{x},返回默认y''值 0");
            return 0;
        }
        /// <summary>
        /// 返回路段位置x的曲率半径
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float road_radius(float x)
        {
            return Mathf.Pow(1 + Mathf.Pow(road_slope(x), 2), 1.5f) / Mathf.Abs(road_bump(x));
        }

        private AutoCode.Tables.SceneRoad.Record get_random_record()
        {

            if (road_records.Count == 0)
            {
                return null;
            }

            int index = Random.Range(0, road_records.Count);

            return road_records[index];
        }

        private void add_curve()
        {
            var rc = get_random_record();
            if (rc == null)
                return;
            AssetBundleManager.instance.load_asset<RoadData>(rc.f_road_data.Item1, rc.f_road_data.Item2, out var road_data);
            if (road_data == null)
            {
                Debug.LogError("获取road数据失败");
            }
            var h = end_pos.y + road_data.points[road_data.points.Count - 1].position.y;
            if (h < Common.Config.current.road_altitude_limit.x || h > Common.Config.current.road_altitude_limit.y)
            {
                add_curve();                //此处有一个隐患问题,如果所有的路面都不能满足高度差需求的话,游戏会卡死
                return;
            }
            var curve = new Curve()
            {
                start_pos = end_pos,
                points = road_data.points,
                curve_sprite = road_data.road_sprite,
                sprite_position = road_data.road_sprite_position,
            };
            curves.Add(curve);
            end_pos += road_data.points[road_data.points.Count - 1].position;           //把路段的最后位置后移

            foreach (var view in views)
            {
                view.add_curve(curve);
            }
        }
    }

    public class Curve
    {
        public Sprite curve_sprite;
        public Vector2 sprite_position;
        public Vector2 start_pos;
        public List<PointData> points = new();
    }

}
