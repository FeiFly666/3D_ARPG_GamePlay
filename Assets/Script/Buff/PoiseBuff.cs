using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Buffs
{
    public class PoiseBuff :Buff
    {
        private float _added;
        public override void OnApply()
        {
            Debug.Log("使用韧性增幅");
            _added = _owner.statusCtrl.maxPoise * (value - 1);

            _owner.statusCtrl.maxPoise += _added;
            if (_added > 0)//只是这样设计，暂时没有降低韧性的情况
                _owner.statusCtrl.RecoverPoise(_added);
        }
        //public virtual void OnUpdate() { }
        public override void OnRemove()
        {
            _owner.statusCtrl.maxPoise -= _added;

            if(_owner.statusCtrl.currentPoise > _owner.statusCtrl.maxPoise)
            {
                _owner.statusCtrl.currentPoise = _owner.statusCtrl.maxPoise;
            }

            Manager.Pool.ReleaseClass(this);
        }
    }
}
