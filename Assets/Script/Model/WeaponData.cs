using Model;
using UnityEngine;

namespace Assets.Model
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string ID = System.Guid.NewGuid().ToString();
        [Header("武器配置")]
        public string weaponName;

        public WeaponType weaponType;


        public GameObject weaponPrefab;
        public GameObject weaponPrefab2;

        public float attackRange;

        public Vector3 positionOffset; // 位置偏移
        public Vector3 rotationOffset; // 角度偏移

        public Vector3 positionOffset2; // 位置偏移
        public Vector3 rotationOffset2; // 角度偏移

        public WeaponCombatData weaponCombatData;
    }
}
