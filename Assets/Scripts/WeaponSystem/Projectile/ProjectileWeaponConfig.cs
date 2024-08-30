using UnityEngine;
using WeaponSystem.Core;

namespace WeaponSystem.Projectile
{
    [CreateAssetMenu(menuName = "Weapon/New projectile weapon config", fileName = "Projectile weapon config")]
    public class ProjectileWeaponConfig : WeaponConfig
    {
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private int _destroyTime = 5000;
        
        public float BulletSpeed => _bulletSpeed;
        public int DestroyTime => _destroyTime;
    }
}