using Assets.Weapon;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Util
{
    public static class CombatCalculate
    {
        public static int CalculateDamage(CharacterBase character, WeaponObject weapon)
        {
            return character.statusCtrl.attack + weapon.data.weaponCombatData.weaponDamage + weapon.currentComboDamageMult;

        }
        public static bool IsInRange(float dis, float range)
        {
            return dis <= range * range;
        }
    }
}
