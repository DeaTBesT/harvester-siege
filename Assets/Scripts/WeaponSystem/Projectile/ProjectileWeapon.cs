using System;
using Mirror;
using UnityEngine;
using Utils.ObjectPool;
using WeaponSystem.Core;

namespace WeaponSystem.Projectile
{
    public class ProjectileWeapon : Weapon
    {
        private const int BULLETS_PRELOAD_COUNT = 10;

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _spawnPivot;

        private GameObjectPool _bulletsPool;
        private ProjectileWeaponConfig _projectileConfig;

        private static bool _isPrefabRegistered;

        public override void Initialize(params object[] objects)
        {
            base.Initialize(objects);

            _bulletsPool = new GameObjectPool(_bulletPrefab, BULLETS_PRELOAD_COUNT);
            _projectileConfig = (ProjectileWeaponConfig)_weaponConfig;
        }

        [Command]
        public override void UseWeapon()
        {
            if (Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + 1f / _weaponConfig.FireRate;

            var bulletObject = _bulletsPool.Get();
            bulletObject.transform.position = _spawnPivot.position;
            bulletObject.transform.rotation = _spawnPivot.rotation;
            NetworkServer.Spawn(bulletObject.gameObject);

            if (bulletObject.TryGetComponent(out Bullet bullet))
            {
                Action onReachTarget = () =>
                {
                    NetworkServer.UnSpawn(bulletObject.gameObject);
                    _bulletsPool.Return(bulletObject.gameObject);
                };

                bullet.Initialize(_entityStats.TeamId, _projectileConfig.Damage, _projectileConfig.BulletSpeed,
                    _projectileConfig.DestroyTime, onReachTarget);
            }
        }

        private GameObject SpawnHandler(SpawnMessage msg) => _bulletsPool.Get();

        private void UnSpawnHandler(GameObject spawned) => _bulletsPool.Return(spawned);
    }
}