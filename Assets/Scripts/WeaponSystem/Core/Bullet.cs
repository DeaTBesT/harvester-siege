using System;
using Core;
using Interfaces;
using Mirror;
using UnityEngine;

namespace WeaponSystem.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : NetworkBehaviour, IInitialize
    {
        [SerializeField] private Rigidbody2D _rigidbody;

        [SyncVar] private int _teamId;
        [SyncVar] private float _damage;
        private float _bulletSpeed;
        private Action _onReachTarget;
        
        public bool IsEnable { get; set; }
        
        protected override void OnValidate()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody2D>();
            }
        }
        public void Initialize(params object[] objects)
        {
            _teamId = (int)objects[0];
            _damage = (float)objects[1];
            _bulletSpeed = (float)objects[2];
            _onReachTarget = objects[4] as Action;

            DestroyDelay((int)objects[3]);
        }

        private void FixedUpdate() => 
            _rigidbody.MovePosition(transform.position + transform.up * _bulletSpeed * Time.fixedDeltaTime);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out EntityStats entityStats))
            {
                if (entityStats.TeamId == _teamId)
                {
                    return;
                }

                entityStats.TakeDamage(_teamId, _damage);
            }

            ForceDestroy();//TODO:fix destroy object
        }

        private void ForceDestroy() => 
            DestroyBulletCmd();

        private void DestroyDelay(int destroyTime) => 
            Invoke(nameof(DestroyDelayCmd), destroyTime);

        [Command(requiresAuthority = false)]
        private void DestroyDelayCmd() => 
            DestroyDelayServer();

        [Server]
        private void DestroyDelayServer() => 
            DestroyBulletServer();

        [Command(requiresAuthority = false)]
        private void DestroyBulletCmd()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            DestroyBulletServer();
        }

        [Server]
        private void DestroyBulletServer()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }
            
            _onReachTarget();
            CancelInvoke();
        }
    }
}