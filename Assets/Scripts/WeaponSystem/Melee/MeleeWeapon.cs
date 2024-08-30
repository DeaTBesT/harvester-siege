using Core;
using Mirror;
using UnityEngine;
using WeaponSystem.Core;

namespace WeaponSystem.Melee
{
    public class MeleeWeapon : Weapon
    {
        [SerializeField] private Transform _castPoint;

        private MeleeConfig _meleeConfig;
        
        public override void Initialize(params object[] objects)
        {
            base.Initialize(objects);

            _meleeConfig = (MeleeConfig)_weaponConfig;
        }

        [Command]
        public override void UseWeapon()
        {
            if (Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + 1f / _weaponConfig.FireRate;

            var hit = Physics2D.Raycast(_castPoint.position,
                _castPoint.up,
                _meleeConfig.AttackDistance,
                _meleeConfig.DamageableLayer);

            if (hit.transform == null)
            {
                return;
            }

            if (hit.transform.TryGetComponent(out EntityStats entityStats))
            {
                entityStats.TakeDamage(_entityStats.TeamId, _weaponConfig.Damage);
            }
        }
    }
}