using UnityEngine;
using WeaponSystem.Core;

namespace WeaponSystem.Melee
{
    [CreateAssetMenu(menuName = "Weapon/New melee weapon config", fileName = "Melee weapon config")]
    public class MeleeConfig : WeaponConfig
    {
        [SerializeField] private float _attackDistance;
        [SerializeField] private LayerMask _damageableLayer;

        public float AttackDistance => _attackDistance;
        public LayerMask DamageableLayer => _damageableLayer;
    }
}