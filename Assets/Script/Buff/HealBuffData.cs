using Assets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    [CreateAssetMenu(fileName = "new Heal Buff", menuName = "Combat/Buff/Heal")]
    public class HealBuffData : BuffData
    {
        public float HealNum = 50;

        public override Buff CreateBuff()
        {
            HealBuff buff = Manager.Pool.GetClass<HealBuff>();

            buff?.OnCreate(HealNum, duration, buffIcon);

            return buff;
        }
    }
}
