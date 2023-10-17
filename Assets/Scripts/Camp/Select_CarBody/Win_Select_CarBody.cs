using Foundation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using World_Formal.CaravanBackPack;

namespace Camp
{
    public class Win_Select_CarBody : MonoBehaviour ,IGarageManagerView
    {
        public CarButton btn; //临时

        CampSceneRoot root;
        CampRootContext ctx;

        public Transform content;

        public List<CarButton> btns = new();

        //==================================================================================================

        void IModelView<GarageManager>.attach(GarageManager owner)
        {
            foreach(var (_, car) in owner.cars)
            {
                var carbtn = Instantiate(btn, content, false);
                carbtn.init(car);

                Button _btn = carbtn.GetComponent<Button>();
                _btn.onClick.RemoveAllListeners();
                _btn.onClick.AddListener(() =>
                {
                    carbtn.onclick();
                    Debug.Log($"已选择车体");
                    ctx.flow = CampRootContext.EN_Flow_State.select_carbody_completed;

                    gameObject.SetActive(false); //关闭窗口
                    root.open_win_select_backpack(car);
                });
                btns.Add(carbtn);
                carbtn.gameObject.SetActive(true);
            }
        }

        void IModelView<GarageManager>.detach(GarageManager owner)
        {
            Destroy(gameObject);        //存疑
        }


        void IModelView<GarageManager>.shift(GarageManager old_owner, GarageManager new_owner)
        {
            
        }

        //====================================================================================================
        public void init()
        {
            root = CampSceneRoot.instance;
            ctx = CampRootContext.instance;
        }

        void IGarageManagerView.add_car(CarBody car)
        {
            var carbtn = Instantiate(btn, content, false);
            carbtn.init(car);
            carbtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                carbtn.onclick();
                Debug.Log($"已选择车体");
                ctx.flow = CampRootContext.EN_Flow_State.select_carbody_completed;

                gameObject.SetActive(false); //关闭窗口
                root.open_win_select_backpack(car);
            });
            btns.Add(carbtn);
            carbtn.gameObject.SetActive(true);
        }

        void IGarageManagerView.remove_car(CarBody car)
        {
            for(int i = 0; i < btns.Count; i++)
            {
               if( btns[i].car == car)
                {
                    Destroy(btns[i].gameObject);
                    btns.RemoveAt(i);
                }

            }
        }
    }
}

