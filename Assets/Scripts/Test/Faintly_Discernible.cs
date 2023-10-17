using Spine.Unity;
using UnityEngine;

public class Faintly_Discernible : MonoBehaviour
{
    public SkeletonAnimation sa;
    public Color color;
    public SpriteRenderer wheel;


    void Update()
    {
        sa.skeleton.SetColor(color);
        wheel.color = color;
    }
}

