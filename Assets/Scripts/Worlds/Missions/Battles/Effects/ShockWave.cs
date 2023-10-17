using Common;
using System.Linq;
using UnityEngine;
using Worlds.Missions.Battles.Enemies;

namespace Worlds.Missions.Battles.Effects
{
    public class ShockWave : MonoBehaviour
    {
        public Transform pic;
        public new CircleCollider2D collider;

        float m_scale;
        bool m_start = false;

        Collider2D[] m_results = new Collider2D[256];

        //==================================================================================================

        public void init()
        {
            m_start = true;
            m_scale = 0;
            Invoke(nameof(shockwave_end), 5);//5秒后失效
        }


        // Update is called once per frame
        void Update()
        {
            if (!m_start) return;
            if (m_scale > 0.5f)
                m_scale = 0;

            m_scale += 0.25f;
            pic.localScale = m_scale * Vector3.one;

            valid_enemies_and_kill();
        }


        /// <summary>
        /// 查找范围内的敌人，并且秒杀
        /// </summary>
        void valid_enemies_and_kill()
        {
            Physics2D.OverlapCircleNonAlloc(transform.position, collider.radius, m_results);
            if (!m_results.Any()) return;

            foreach (var r in m_results.Where(t => t != null))
            {
                if (r.TryGetComponent<IEnemyView>(out var view))
                {
                    var cell = view.cell;
                    cell.dead();
                }
            }
        }


        void shockwave_end()
        {
            DestroyImmediate(gameObject);
        }
    }
}

