using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadData : ScriptableObject
{
    public List<PointData> points = new List<PointData>();

    public Sprite road_sprite;

    public Vector2 road_sprite_position;
}

[System.Serializable]
public class PointData
{
    public Vector2 position,left_position,right_position;
}
