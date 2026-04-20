using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(fileName = "NewCharacter", menuName = "Combat/CharacterData")]
public class CharacterData : ScriptableObject
{
    [SerializeField] public string ID = System.Guid.NewGuid().ToString(); 
    [SerializeField] private string characterName;
    [SerializeField] private int fullHP;
    [SerializeField] private int atk;
    [SerializeField] private int def;
    [SerializeField] private int maxStamina;
    [SerializeField] private int maxPoise;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float detectDistance;

    public string Name => characterName;
    public int FullHP => fullHP;
    public int ATK => atk;
    public int DEF => def;
    public int MaxStamina => maxStamina;
    public int MaxPoise => maxPoise;
    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float DetectDistance => detectDistance;
}
