using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public PlayerController player;

    public List<EnemyController> enemies = new List<EnemyController>();

    public void RegisterCharacter(CharacterBase character)
    {
        if (character == null) return;

        if(character is PlayerController player)
        {
            this.player = player;
        }
        else if( character is EnemyController enemy)
        {
            if(!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }
    }
}
