using Assets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    [CreateAssetMenu(fileName = "new Poise Buff", menuName = "Combat/Buff/Poise")]
    public class PoiseBuffData : BuffData
    {
        public float PoiseAddMult = 1.2f;

        public override Buff CreateBuff()
        {
            PoiseBuff buff = Manager.Pool.GetClass<PoiseBuff>();

            buff?.OnCreate(PoiseAddMult, duration, buffIcon);

            return buff;
        }
    }
}
