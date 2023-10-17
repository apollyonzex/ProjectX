using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Anm_Controller : MonoBehaviour
{
    public float velocity;
    public Animator animator;

    List<string> m_exclusion_prms_names = new()
    {
        { "is_stop" },
        { "is_run" },
    };

    //==================================================================================================

    // Update is called once per frame
    void Update()
    {
        if (velocity == 0)
        {
            select_exclusion_prm("is_stop");
            return;
        }

        if (velocity > 0)
        {
            select_exclusion_prm("is_run");
        }
    }


    void select_exclusion_prm(string name)
    {
        foreach (var e in m_exclusion_prms_names.Where(e => e != name))
        {
            animator.SetBool(e, false);
        }

        animator.SetBool(name, true);
    }
}
