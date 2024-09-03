using System;
using Core;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerController : EntityController
    {
        private EntityStats _entityStats;
        private EntityMovementController _entityMovementController;
        private EntityWeaponController _entityWeaponController;
        private Collider2D _collider;
        private Transform _graphics;

        private void Start()
        {
            if (isServer)
            {
                return;
            }

            LoadResources(NetworkClient.localPlayer.connectionToClient);
        }

        [Command(requiresAuthority = false)]
        private void LoadResources(NetworkConnectionToClient conn) =>
            LoadResourceServer(conn);

        [Server]
        private void LoadResourceServer(NetworkConnectionToClient conn)
        {
            LoadResourceRpc(conn, 
                _entityStats.IsEnable,
                _entityMovementController.IsEnable,
                _entityWeaponController.IsEnable,
                _collider.enabled,
                _graphics.gameObject.activeSelf);
        }

        [TargetRpc]
        private void LoadResourceRpc(NetworkConnectionToClient target, bool entityStatsState,
            bool entityMovementControllerState, bool entityWeaponControllerState, bool colliderState,
            bool graphicsState)
        {
            _entityStats.IsEnable = entityStatsState;
            _entityMovementController.IsEnable = entityMovementControllerState;
            _entityWeaponController.IsEnable = entityWeaponControllerState;
            _collider.enabled = colliderState;
            _graphics.gameObject.SetActive(graphicsState);
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
    }
}