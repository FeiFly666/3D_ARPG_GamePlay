using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Combat
{
    public interface IInterruptible
    {
        public void TriggerHit();//触发中断
        public void TriggerBlockHit();//触发格挡受击
        public void TriggerStagger();//触发硬直
        public bool TryCancelHit();//取消受击


    }
}
