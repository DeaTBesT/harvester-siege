using Interfaces;
using Mirror;
using UnityEngine;

namespace InputModule
{
    public sealed class InputHandler : NetworkBehaviour, IInitialize
    {
        private IInput _input;

        private bool _isIgnoreLocalPlayer;
        
        public bool IsEnable { get; set; }

        public void Initialize(params object[] objects) =>
            _input = objects[0] as IInput;

        private void UpdateHandler()
        {
            _input.InteractHandler();
            _input.EscapeHandler();

            if (IsEnable)
            {
                _input.MouseHandler();
                _input.AttackOnceHandler();
                _input.AttackHandler();
            }
        }

        private void FixedUpdateHandler()
        {
            if (!IsEnable)
            {
                return;
            }

            _input.MoveHandler();
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            UpdateHandler();
        }

        private void FixedUpdate()
        {
            if ((!isLocalPlayer) && (!_isIgnoreLocalPlayer))
            {
                return;
            }

            FixedUpdateHandler();
        }

        public void SetEnableLocal(bool isEnable) =>
            SetEnableLocalCmd(isEnable);

        [Command(requiresAuthority = false)]
        private void SetEnableLocalCmd(bool isEnable) =>
            SetEnableLocalRpc(isEnable);

        [ClientRpc]
        private void SetEnableLocalRpc(bool isEnable)
        {
            if (!isLocalPlayer)
            {
                return;
            }
            
            IsEnable = isEnable;
        }

        public void SetEnableByNetId(NetworkIdentity netId, bool isEnable) => 
            SetEnableByNetIdCmd(netId, isEnable);

        [Command(requiresAuthority = false)]
        private void SetEnableByNetIdCmd(NetworkIdentity netId, bool isEnable) => 
            SetEnableByNetIdRpc(netId, isEnable);

        [ClientRpc]
        private void SetEnableByNetIdRpc(NetworkIdentity netId, bool isEnable)
        {
            if (!netId.isLocalPlayer)
            {
                return;
            }

            IsEnable = isEnable;
        }

        public void SetEnableGlobal(bool isEnable) => 
            SetEnableGlobalCmd(isEnable);

        [Command(requiresAuthority = false)]
        private void SetEnableGlobalCmd(bool isEnable) => 
            SetEnableGlobalRpc(isEnable);

        [ClientRpc]
        private void SetEnableGlobalRpc(bool isEnable) => 
            IsEnable = isEnable;

        public void SetIgnoreLocalPlayer(bool isIgnoreLocalPlayer) => 
            SetIgnoreLocalPlayerCmd(isIgnoreLocalPlayer);

        [Command(requiresAuthority = false)]
        private void SetIgnoreLocalPlayerCmd(bool isIgnoreLocalPlayer) => 
            SetIgnoreLocalPlayerRpc(isIgnoreLocalPlayer);

        [ClientRpc]
        private void SetIgnoreLocalPlayerRpc(bool isIgnoreLocalPlayer) => 
            _isIgnoreLocalPlayer = isIgnoreLocalPlayer;
    }
}