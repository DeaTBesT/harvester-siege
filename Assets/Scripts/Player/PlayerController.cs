using Core;
using Interfaces;
using Managers;
using Mirror;
using Player.CustomSerialization;
using UnityEngine;

namespace Player
{
    public class PlayerController : EntityController, INetworkLoad
    {
        private EntityStats _entityStats;
        private EntityMovementController _entityMovementController;
        private EntityWeaponController _entityWeaponController;
        private Collider2D _collider;
        private Transform _graphics;

        public override void OnStartServer() =>
            NetworkLoadManager.Instance.AddLoader(this);
        
        public override void OnStopServer() =>
            NetworkLoadManager.Instance.RemoveLoader(this);

        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn)
        {
            var data = new PlayerControllerData(_entityStats.IsEnable,
                _entityMovementController.IsEnable,
                _entityWeaponController.IsEnable,
                _collider.enabled,
                _graphics.gameObject.activeSelf);

            var writer = new NetworkWriter();
            writer.WritePlayerControllerData(data);
            var writerData = writer.ToArray();

            LoadDataRpc(conn, writerData);
        }

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData)
        {
            var reader = new NetworkReader(writerData);
            var data = reader.ReadPlayerControllerData();

            _entityStats.IsEnable = data.IsEnableEntityStats;
            _entityMovementController.IsEnable = data.IsEnableEntityMovementController;
            _entityWeaponController.IsEnable = data.IsEnableEntityWeaponController;
            _collider.enabled = data.IsEnableCollider;
            _graphics.gameObject.SetActive(data.IsEnableEntityStats);
        }

        public override void Initialize(params object[] objects)
        {
            _entityStats = objects[0] as EntityStats;
            _entityMovementController = objects[1] as EntityMovementController;
            _entityWeaponController = objects[2] as EntityWeaponController;
            _collider = objects[3] as Collider2D;
            _graphics = objects[4] as Transform;
        }

        public override void ActivateEntity() =>
            ActivateEntityCmd();

        [Command(requiresAuthority = false)]
        private void ActivateEntityCmd() =>
            ActivateEntityRpc();

        [ClientRpc]
        private void ActivateEntityRpc()
        {
            _entityStats.IsEnable = true;
            _entityMovementController.IsEnable = true;
            _entityWeaponController.IsEnable = true;
            _collider.enabled = true;
            _graphics.gameObject.SetActive(true);
        }

        public override void DiactivateEntity() =>
            DiactivateEntityCmd();

        [Command(requiresAuthority = false)]
        private void DiactivateEntityCmd() =>
            DiactivateEntityRpc();

        [ClientRpc]
        private void DiactivateEntityRpc()
        {
            _entityStats.IsEnable = false;
            _entityMovementController.IsEnable = false;
            _entityWeaponController.IsEnable = false;
            _collider.enabled = false;
            _graphics.gameObject.SetActive(false);
        }

        public override void ChangePosition(Vector2 newPosition) =>
            ChangePositionCmd(newPosition);

        [Command(requiresAuthority = false)]
        private void ChangePositionCmd(Vector2 newPosition) =>
            ChangePositionRpc(newPosition);

        [ClientRpc]
        private void ChangePositionRpc(Vector2 newPosition) =>
            transform.position = newPosition;

        public override void ActivateMoveEntity() => 
            ActivateMoveEntityCmd();

        [Command(requiresAuthority = false)]
        private void ActivateMoveEntityCmd() => 
            ActivateMoveEntityRpc();

        [ClientRpc]
        private void ActivateMoveEntityRpc()
        {
            _entityMovementController.IsEnable = true;
            _entityWeaponController.IsEnable = true;
        }
        
        public override void DiactivateMoveEntity() => 
            DiactivateMoveEntityCmd();

        [Command(requiresAuthority = false)]
        private void DiactivateMoveEntityCmd() => 
            DiactivateMoveEntityRpc();

        [ClientRpc]
        private void DiactivateMoveEntityRpc()
        {
            _entityMovementController.IsEnable = false;
            _entityWeaponController.IsEnable = false;
        }
    }
}