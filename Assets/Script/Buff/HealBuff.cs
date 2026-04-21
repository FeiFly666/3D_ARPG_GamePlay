using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    public class HealBuff : Buff
    {
        public override void OnApply()
        {
            Debug.Log("使用生命增幅");
            if(duration == 0 )
                _owner.statusCtrl.RecoverHP(value);
        }
        public override void OnUpdate()
        {
            if (duration != 0)
                _owner.statusCtrl.RecoverHP(value / duration * Time.deltaTime);
        }
        public override void OnRemove()
        {
            Manager.Pool.ReleaseClass(this);
        }
    }
}
