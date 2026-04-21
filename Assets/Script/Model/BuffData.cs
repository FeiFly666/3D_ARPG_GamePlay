using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Buffs;

namespace Assets.Model
{
    public abstract class BuffData : ScriptableObject
    {
        public string buffName;

        public float duration;

        public Sprite buffIcon;
        public abstract Buff CreateBuff();
    }
}
