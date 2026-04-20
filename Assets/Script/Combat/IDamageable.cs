using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Combat
{
    public interface IDamageable
    {
        void TakeDamage(int damage, int poiseDamage, CharacterBase attacker);
    }
}
