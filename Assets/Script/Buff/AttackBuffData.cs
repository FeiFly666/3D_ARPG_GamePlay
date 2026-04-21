using Assets.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    [CreateAssetMenu(fileName = "new Attack Buff", menuName ="Combat/Buff/Attack")]
    public class AttackBuffData : BuffData
    {
        public float attackAddMult = 1.2f;

        public override Buff CreateBuff()
        {
            AttackBuff buff = Manager.Pool.GetClass<AttackBuff>();

            buff?.OnCreate(attackAddMult, duration, buffIcon);

            return buff;
        }
    }
}
