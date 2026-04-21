using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace Assets.Buffs
{
    public abstract class Buff
    {
        protected float duration;
        protected float value;
        protected CharacterBase _owner;
        public float timer;
        public Sprite buffIcon;

        public void OnCreate(float value, float duration, Sprite icon)
        {
            this.value = value;
            this.duration = duration;
            this.buffIcon = icon;
        }

        public void Init(CharacterBase owner)
        {
            this._owner = owner;

            this.timer = this.duration;

            OnApply();
        }

        public virtual void OnApply() { }
        public virtual void OnUpdate() { }
        public virtual void OnRemove() { }

        public bool Update(float delta)
        {
            timer -= delta;
            OnUpdate();

            return timer >= 0;
        }
        public void Remove()
        {
            OnRemove();
        }
    }
}
