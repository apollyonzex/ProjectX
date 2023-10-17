using Spine.Unity;
using UnityEngine;

public class Enemy_SpineView : MonoBehaviour
{
    public SkeletonAnimation anim;

    string m_name;
    bool m_loop;

    bool is_new_anim = false;
    bool is_lock = false;

    //==================================================================================================

    public void set_anim(string name, bool loop)
    {
        m_name = name;
        m_loop = loop;

        is_new_anim = true;
    }


    private void Update()
    {
        if (anim == null) return;
        if (!is_new_anim) return;
        if (is_lock) return;

        if (anim.AnimationName != m_name)
        {
            var clip = anim.state.Data.SkeletonData.FindAnimation(m_name);
            if (clip != null)
            {
                anim.state.SetAnimation(0, clip, m_loop);
            }
        }

        is_new_anim = false;
    }
}
