using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World_Formal.UI
{
    public class progress_monster_tip : MonoBehaviour
    {
        public GameObject upcoming;
        public GameObject passed;

        //==================================================================================================

        public void set_passed(bool is_passed = false)
        {
            passed.SetActive(is_passed);
            upcoming.SetActive(!is_passed);
        }
    }
}

