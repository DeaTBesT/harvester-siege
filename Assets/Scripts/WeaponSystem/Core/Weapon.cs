using Core;
using Interfaces;
using Mirror;
using UnityEngine;

namespace WeaponSystem.Core
{
    public abstract class Weapon : NetworkBehaviour, IInitialize
    {
        [SerializeField] protected WeaponConfig _weaponConfig;

        protected float _nextAttackTime;

        protected EntityStats _entityStats;

        public WeaponConfig GetWeaponConfig => _weaponConfig;

        public bool IsEnable { get; set; }
        
        public virtual void Initialize(params object[] objects) => 
            _entityStats = objects[0] as EntityStats;

        public virtual void Deinitialize()
        {
        }

        public abstract void UseWeapon();
    }
}