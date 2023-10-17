using UnityEngine;


namespace Worlds.Missions.Battles.Enemies.BehaviourTrees
{
    public interface IEnemy_Interaction_Target
    {
        Vector2 position { get; }
        Vector2 velocity { get; }
        Vector2 collider { get; }
        Vector2 collider_center { get; }
        string name { get; }
        int current_hp { get; set; }
    }

}

