﻿using CalcExpr;
using DeviceGraph;
using DeviceViews;
using UnityEngine;

namespace Devices {
    public class DeviceVessel : DeviceComponent{

        private int m_max_value;
        private int m_value;
        private DeviceVesselNode m_node;

        public override string name => m_node.module_id;

        public override DeviceNode graph_node => m_node;

        [ExprConst]
        public int value {
            get => m_value;
            set => m_value = Mathf.Clamp(value, 0, m_max_value);
        }

        [ExprConst]
        public int max_value => m_max_value;


        public DeviceVessel(DeviceVesselNode node, int value, int max_value) {
            m_node = node;
            m_max_value = Mathf.Max(1, max_value);
            m_value = Mathf.Clamp(value, 0, m_max_value);
        }
    }
}
