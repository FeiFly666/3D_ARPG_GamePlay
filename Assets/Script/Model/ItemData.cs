using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Model
{
    [CreateAssetMenu(fileName ="New Item", menuName = "Item")]
    public class ItemData : ScriptableObject
    {
        public string Name;
        public string Description;

        public float coldDown;

        public Sprite icon;

        public BuffData Buff;
    }
}
