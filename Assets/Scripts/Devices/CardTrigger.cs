

using DeviceGraph;

namespace Devices {
    public class CardTrigger :DeviceComponent {

        private CardTriggerNode m_node;

        public CardTrigger(CardTriggerNode node) {
            m_node = node;
        }

        public override string name => m_node.module_id;
        public override DeviceNode graph_node => m_node;

        public bool trigger(DeviceContext ctx) {
            m_node.set_position(Worlds.Missions.Battles.BattleSceneRoot.instance.battlecardMgr.use_card_position);
            m_node.output?.invoke(ctx);
            return ctx.current_bool;
        }
    }
}
