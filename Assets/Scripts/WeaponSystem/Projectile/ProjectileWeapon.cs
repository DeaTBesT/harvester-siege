using System;
using Managers;
using Mirror;
using UnityEngine;
using WeaponSystem.Core;

namespace WeaponSystem.Projectile
{
    public class ProjectileWeapon : Weapon
    {
        [SerializeField] private NetworkIdentity _bulletPrefab;
        [SerializeField] private Transform _spawnPivot;

        private PrefabPool _prefabPool;
        private ProjectileWeaponConfig _projectileConfig;

        public override void Initialize(params object[] objects)
        {
            base.Initialize(objects);

            _projectileConfig = (ProjectileWeaponConfig)_weaponConfig;
            _prefabPool = PrefabPoolManager.Instance.GetPool(_projectileConfig.TypePool);
        }

        [Command]
        public override void UseWeapon()
        {
            if (Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + 1f / _weaponConfig.FireRate;

            var bulletObject = _prefabPool.Get();
            bulletObject.transform.position = _spawnPivot.position;
            bulletObject.transform.rotation = _spawnPivot.rotation;
            NetworkServer.Spawn(bulletObject.gameObject);

            if (bulletObject.TryGetComponent(out Bullet bullet))
            {
                Action onReachTarget = () =>
                {
                    NetworkServer.UnSpawn(bulletObject.gameObject);
                    _prefabPool.Return(bulletObject.gameObject);
                };

                bullet.Initialize(_entityStats.TeamId, _projectileConfig.Damage, _projectileConfig.BulletSpeed,
                    _projectileConfig.DestroyTime, onReachTarget);
            }
        }
    }
}