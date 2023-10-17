using System.Collections;
using UnityEngine;

namespace BehaviourFlow {
    public class BTEvent {

        public uint ticket { get; private set; }
        public bool rised { get; private set; }

        public BTEvent(bool rised) {
            this.rised = rised;
        }

        public void reset() {
            if (rised) {
                m_last_rised = ticket++;
                rised = false;
            }
        }

        public void rise(bool auto_reset = true) {
            if (auto_reset) {
                m_last_rised = ticket++;
                rised = false;
            } else if (!rised) {
                m_last_rised = null;
                ++ticket;
                rised = true;
            }
        }

        public bool poll_wait(uint ticket) {
            if (rised) {
                return true;
            }
            if (m_last_rised.HasValue && m_last_rised.Value >= ticket) {
                return true;
            }
            return false;
        }

        private uint? m_last_rised;
    }
}