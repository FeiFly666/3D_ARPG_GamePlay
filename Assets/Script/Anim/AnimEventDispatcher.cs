using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Anim
{
    public class AnimEventDispatcher : MonoBehaviour
    {
        public System.Action onRightAttackStart;
        public System.Action onRightAttackEnd;

        public System.Action onLeftAttackStart;
        public System.Action onLeftAttackEnd;

        public void StartHit()
        {
            onRightAttackStart?.Invoke();
        }
        public void EndHit()
        {
            onRightAttackEnd?.Invoke();
        }
        public void StartLeftHit()
        {
            onLeftAttackStart?.Invoke();
        }
        public void EndLeftHit()
        {
            onLeftAttackEnd?.Invoke();
        }

        private void OnDestroy()
        {
            onRightAttackStart = null;
            onRightAttackEnd = null;
        }
    }
}
