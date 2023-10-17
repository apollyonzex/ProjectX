using Common_Formal;
using AutoCode.Tables;
using System.Collections.Generic;
using World_Formal.Caravans;
using Common_Formal.DS;

namespace World_Formal
{
    public class DB : Singleton<DB>
    {
        Device m_device;

        public Device device
        {
            get
            {
                if(m_device == null)
                {
                    EX_Utility.try_load_table("device", out m_device);
                }
                return m_device;
            }
        }

        Item m_items;
        public Item items
        {
            get
            {
                if (m_items == null)
                {
                    EX_Utility.try_load_table("item", out m_items);
                }
                return m_items;
            }
        }


        Body m_caravan_body;
        public Body caravan_body
        {
            get
            {
                if (m_caravan_body == null)
                {
                    EX_Utility.try_load_table("body", out m_caravan_body);
                }
                return m_caravan_body;
            }
        }


        #region 动画
        SpineWorldCaravan m_spineWorldCaravan;
        public SpineWorldCaravan spineWorldCaravan
        {
            get
            {
                if (m_spineWorldCaravan == null)
                {
                    EX_Utility.try_load_table("spine_world_caravan", out m_spineWorldCaravan);
                }
                return m_spineWorldCaravan;
            }
        }
        Dictionary<uint, SpineDS> m_spineWorldCaravan_info;
        public Dictionary<uint, SpineDS> spineWorldCaravan_info
        {
            get
            {
                if (m_spineWorldCaravan_info == null)
                {
                    Dictionary<uint, SpineDS> dic = new();
                    foreach (var e in spineWorldCaravan.records)
                    {
                        SpineDS ds = new()
                        {
                            name = "",
                            init = e.f_init,
                            idle = e.f_idle,
                            run = e.f_run,
                            jump = e.f_jump,
                            brake = e.f_brake,
                            squrt = e.f_squrt,
                            jumping = e.f_jumping,
                            land = e.f_land
                        };
                        dic.Add(e.f_id, ds);
                        m_spineWorldCaravan_info = dic;
                    }
                }
                return m_spineWorldCaravan_info;
            }
        }


        SpineWorldDevice m_spineWorldDevice;
        public SpineWorldDevice spineWorldDevice
        {
            get
            {
                if (m_spineWorldDevice == null)
                {
                    EX_Utility.try_load_table("spine_world_device", out m_spineWorldDevice);
                }
                return m_spineWorldDevice;
            }
        }
        Dictionary<uint, SpineDS> m_spineWorldDevice_info;
        public Dictionary<uint, SpineDS> spineWorldDevice_info
        {
            get
            {
                if (m_spineWorldDevice_info == null)
                {
                    Dictionary<uint, SpineDS> dic = new();
                    foreach (var e in spineWorldDevice.records)
                    {
                        SpineDS ds = new()
                        {
                            name = "",
                            init = e.f_init,
                            idle = e.f_idle,
                            run = e.f_run,
                            jump = e.f_jump,
                            brake = e.f_brake,
                            squrt = e.f_squrt,
                            jumping = e.f_jumping,
                            land = e.f_land
                        };
                        dic.Add(e.f_id, ds);
                        m_spineWorldDevice_info = dic;
                    }
                }

                return m_spineWorldDevice_info;
            }
        }


        SpineBattleCaravan m_spineBattleCaravan;
        public SpineBattleCaravan spineBattleCaravan
        {
            get
            {
                if (m_spineBattleCaravan == null)
                {
                    EX_Utility.try_load_table("spine_battle_caravan", out m_spineBattleCaravan);
                }
                return m_spineBattleCaravan;
            }
        }
        Dictionary<uint, SpineDS> m_spineBattleCaravan_info;
        public Dictionary<uint, SpineDS> spineBattleCaravan_info
        {
            get
            {
                if (m_spineBattleCaravan_info == null)
                {
                    Dictionary<uint, SpineDS> dic = new();
                    foreach (var e in spineBattleCaravan.records)
                    {
                        SpineDS ds = new()
                        {
                            name = "",
                            init = e.f_init,
                            idle = e.f_idle,
                            run = e.f_run,
                            jump = e.f_jump,
                            brake = e.f_brake,
                            squrt = e.f_squrt,
                            jumping = e.f_jumping,
                            land = e.f_land
                        };
                        dic.Add(e.f_id, ds);
                    }
                    m_spineBattleCaravan_info = dic;
                }
                
                return m_spineBattleCaravan_info;
            }
        }


        SpineBattleDevice m_spineBattleDevice;
        public SpineBattleDevice spineBattleDevice
        {
            get
            {
                if (m_spineBattleDevice == null)
                {
                    EX_Utility.try_load_table("spine_battle_device", out m_spineBattleDevice);
                }
                return m_spineBattleDevice;
            }
        }
        Dictionary<uint, SpineDS> m_spineBattleDevice_info;
        public Dictionary<uint, SpineDS> spineBattleDevice_info
        {
            get
            {
                if (m_spineBattleDevice_info == null)
                {
                    Dictionary<uint, SpineDS> dic = new();
                    foreach (var e in spineBattleDevice.records)
                    {
                        SpineDS ds = new()
                        {
                            name = "",
                            init = e.f_init,
                            idle = e.f_idle,
                            run = e.f_run,
                            jump = e.f_jump,
                            brake = e.f_brake,
                            squrt = e.f_squrt,
                            jumping = e.f_jumping,
                            land = e.f_land
                        };
                        dic.Add(e.f_id, ds);
                    }
                    m_spineBattleDevice_info = dic;
                }
                
                return m_spineBattleDevice_info;
            }
        }
        #endregion


        #region 战斗
        Contents m_contents;
        public Contents contents
        {
            get
            {
                if (m_contents == null)
                {
                    EX_Utility.try_load_table("contents", out m_contents);
                }
                return m_contents;
            }
        }
        Dictionary<(uint,uint), Contents.Record> m_content_dic;
        public Dictionary<(uint, uint), Contents.Record> content_dic
        {
            get
            {
                if (m_content_dic == null)
                {
                    Dictionary<(uint, uint), Contents.Record> dic = new();
                    foreach (var r in contents.records)
                    {
                        dic.Add((r.f_id, r.f_journey_difficult), r);
                    }
                    m_content_dic = dic;
                }
                
                return m_content_dic;
            }
        }


        MonsterGroup m_monster_group;
        public MonsterGroup monster_group
        {
            get
            {
                if (m_monster_group == null)
                {
                    EX_Utility.try_load_table("monster_group", out m_monster_group);
                }
                return m_monster_group;
            }
        }
        Dictionary<(uint, uint), MonsterGroup.Record> m_monster_group_dic;
        public Dictionary<(uint, uint), MonsterGroup.Record> monster_group_dic
        {
            get
            {
                if (m_monster_group_dic == null)
                {
                    Dictionary<(uint, uint), MonsterGroup.Record> dic = new();
                    foreach (var r in monster_group.records)
                    {
                        dic.Add((r.f_group_id, r.f_sub_id), r);
                    }
                    m_monster_group_dic = dic;
                }

                return m_monster_group_dic;
            }
        }


        Battle m_battle;
        public Battle battle
        {
            get
            {
                if (m_battle == null)
                {
                    EX_Utility.try_load_table("battle", out m_battle);
                }
                return m_battle;
            }
        }


        MonsterNormal m_monster_normal;
        public MonsterNormal monster_normal
        {
            get
            {
                if (m_monster_normal == null)
                {
                    EX_Utility.try_load_table("monster_normal", out m_monster_normal);
                }
                return m_monster_normal;
            }
        }


        Projectile m_projectile;
        public Projectile projectile
        {
            get
            {
                if(m_projectile == null)
                {
                    EX_Utility.try_load_table("projectile", out m_projectile);
                }
                return m_projectile;
            }
        }
        #endregion
    }
}

