using Spine;
using Spine.Unity;
using UnityEngine;

public class Test_Anm : MonoBehaviour
{
    public SkeletonAnimation anm;
    public float stone_act_scale_x = 1;

    [Range(1,4)]
    public float light_scale = 1;

    Bone m_stone_act;
    Bone m_light;
    TransformConstraint m_stone_scale;

    public float s = 1;

    //==================================================================================================

    // Start is called before the first frame update
    void Start()
    {
        m_stone_act = anm.skeleton.FindBone("stone_act");
        m_light = anm.skeleton.FindBone("light");
        m_stone_scale = anm.skeleton.FindTransformConstraint("stone_scale") ;
    }


    // Update is called once per frame
    void Update()
    {
        m_stone_act.ScaleX = stone_act_scale_x;

        m_light.ScaleX = light_scale;
        m_light.ScaleY = light_scale;

        m_stone_scale.MixScaleX = s;
    }


    public void switch_anm_state()
    {
        var clip = anm.state.Data.SkeletonData.FindAnimation("landing");
        anm.state.SetAnimation(0, clip, false);
    }
}
