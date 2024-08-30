using System.Linq;
using Interfaces;
using Mirror;
using UnityEngine;
using WeaponSystem.Core;

namespace Core
{
    public abstract class EntityWeaponController : NetworkBehaviour, IInitialize, IDeinitialize
    {
        [SerializeField] protected Weapon[] _weaponsContainer;
        
        protected Weapon _currentWeapon;

        [field: SerializeField] public virtual bool IsEnable { get; set; } = true;
        
        public virtual void Initialize(params object[] objects)
        {
            foreach (var weapon in _weaponsContainer)
            {
                weapon.Initialize(objects);
                weapon.gameObject.SetActive(false);
            }
        }
        
        public virtual void Deinitialize()
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void UseWeapon()
        {
            _currentWeapon.UseWeapon();
        }

        public virtual void EquipWeapon(WeaponConfig weaponConfig)
        {
            _currentWeapon = _weaponsContainer.FirstOrDefault(weapon => weapon.GetWeaponConfig == weaponConfig);

            if (_currentWeapon == null)
            {
#if UNITY_EDITOR
                Debug.LogError("None weapon in container");
#endif
                return;
            }
            
            _currentWeapon.gameObject.SetActive(true);
        }

        public virtual void UnequipWeapon()
        {
            if (_currentWeapon == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Equipped weapon is null");
#endif
                return;
            }

            _currentWeapon.gameObject.SetActive(false);
        }
    }
}