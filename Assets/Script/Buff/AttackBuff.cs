using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    public class AttackBuff : Buff
    {
        private float _added;
        public override void OnApply()
        {
            Debug.Log("使用攻击增幅");
            _added = _owner.statusCtrl.attack * (value - 1);

            _owner.statusCtrl.attack += _added;
        }
        //public virtual void OnUpdate() { }
        public override void OnRemove()
        {
            _owner.statusCtrl.attack -= _added;

            Manager.Pool.ReleaseClass(this);
        }
    }
}
