

using Worlds.Missions.Battles.Enemies.BehaviourTrees;

namespace Worlds.Missions.Battles
{
    [System.Serializable]
    public class Enemy_BT : BehaviourFlow.BehaviourTree
    {
        public override System.Type context_type => typeof(EnemyBrainProxy);
    }


}


