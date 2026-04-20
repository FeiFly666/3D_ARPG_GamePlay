using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Model
{
    public enum EffectAttackType
    {
        NotAttack,
        Block,
        Circle,
        Ring
    }
    [Serializable]
    public class EffectData
    {
        public GameObject VFXPrefab;

        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = new Vector3(1, 1, 1);
    }
}
