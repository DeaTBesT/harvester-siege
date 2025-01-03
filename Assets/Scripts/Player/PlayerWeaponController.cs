using Core;
using Interfaces;
using UnityEngine;
using WeaponSystem.Core;

namespace Player
{
    public class PlayerWeaponController : EntityWeaponController
    {
        [SerializeField] private WeaponConfig _startWeapon;
        
        private IInput _inputModule;
        
        public override void Initialize(params object[] objects)
        {
            var entityStats = objects[1] as EntityStats;

            base.Initialize(entityStats);
            
            EquipWeapon(_startWeapon);
            _inputModule = objects[0] as IInput;
            
            if (_inputModule == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Input module is null");
#endif
                return;
            }
            
            _inputModule.OnAttack += UseWeapon;
        }

        public override void Deinitialize(params object[] objects)
        {
            if (_inputModule == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Input module is null");
#endif
                return;
            }
            
            _inputModule.OnAttack -= UseWeapon;

            foreach (var weapon in _weaponsContainer)
            {
                weapon.Deinitialize();
            }
        }
        
        public override void UseWeapon()
        {
            if (_currentWeapon == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Weapon is null");
#endif
                return;
            }

            if (!IsEnable)
            {
                return;
            }
            
            _currentWeapon.UseWeapon();
        }
    }
}