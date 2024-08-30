using Interfaces;
using Mirror;
using UnityEngine;

namespace Core
{
    public class CameraController : NetworkBehaviour, IInitialize
    {
        private const float CAMERA_Z_POSITION = -10f;

        [SerializeField] private float _cameraSmooth = 5f;

        private Camera _camera;

        private Transform _target;

        public bool IsEnable { get; set; }

        public void Initialize(params object[] objects)
        {
            _camera = objects[0] as Camera;
            _target = objects[1] as Transform;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (_camera == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Camera is null");
#endif
                return;
            }

            _camera.transform.position = Vector3.Lerp(_camera.transform.position,
                _target.position + Vector3.forward * CAMERA_Z_POSITION, _cameraSmooth * Time.fixedDeltaTime);
        }

        public void ChangeTarget(NetworkIdentity target) => 
            ChangeTargetCmd(target);

        [Command(requiresAuthority = false)]
        private void ChangeTargetCmd(NetworkIdentity target) =>
            ChangeTargetRpc(target);   
        
        [ClientRpc]
        private void ChangeTargetRpc(NetworkIdentity target) => 
            _target = target.transform;
    }
}