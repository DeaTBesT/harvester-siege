using Core;
using Enums;
using Interfaces;
using Mirror;
using UnityEngine;

namespace Player
{
    public class PlayerInteractionController : EntityInteractionController
    {
        [SerializeField] private float _interactDistance;
        [SerializeField] private LayerMask _interactLayer;
        [SerializeField] private Transform _interactPoint;

        private Vector2 _mousePosition;

        [SyncVar] private NetworkIdentity _currentInteractable;
        
        public override void Initialize(params object[] objects)
        {
            base.Initialize(objects);

            if (_inputModule != null)
            {
                _inputModule.OnMousePosition += OnMousePosition;
            }
        }

        public override void Deinitialize(params object[] objects)
        {
            base.Deinitialize();

            if (_inputModule != null)
            {
                _inputModule.OnMousePosition -= OnMousePosition;
            }

            if (_currentInteractable != null)
            {
                if (_currentInteractable.TryGetComponent(out IInteractable interactable))
                {
                    interactable.ForceFinishInteract(this);
                    ToggleInteractionEvents();
                }
            }
        }

        private void OnMousePosition(Vector3 mousePosition) =>
            _mousePosition = mousePosition;

        public override void OnInteract()
        {
            var hit = InteractRay();

            if (hit.transform == null)
            {
                return;
            }

            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.TryInteract(this, OnEndInteract))
                {
                    switch (interactable.TypeInteract)
                    {
                        case InteractType.OneTime:
                        {
                            SetCurrentInteractable(null);
                        }
                            break;
                        case InteractType.Toggle:
                        {
                            SetCurrentInteractable(interactable.NetIdentity);
                            ToggleInteractionEvents();
                        }
                            break;
                        case InteractType.Holding:
                        {
                            SetCurrentInteractable(interactable.NetIdentity);
                        }
                            break;
                        default:
                        {
#if UNITY_EDITOR
                            Debug.LogError("None interactable type");
#endif
                        }
                            break;
                    }
                }
            }
        }

        public override void OnInteractUp()
        {
            if (_currentInteractable == null)
            {
                return;
            }

            if (_currentInteractable.TryGetComponent(out IInteractable interactable))
            {
                interactable.StopHolding();
            }
        }

        private RaycastHit2D InteractRay()
        {
            var currentScene = gameObject.scene;
            var interactDirection = _mousePosition - (Vector2)_interactPoint.position;
            
            return currentScene.GetPhysicsScene2D().Raycast(_interactPoint.position,
                interactDirection,
                _interactDistance,
                _interactLayer);
        }

        private void SetCurrentInteractable(NetworkIdentity netIdentity) =>
            SetCurrentInteractableCmd(netIdentity);
        
        [Command]
        private void SetCurrentInteractableCmd(NetworkIdentity netIdentity) =>
            SetCurrentInteractableRpc(netIdentity);
        
        [ClientRpc]
        private void SetCurrentInteractableRpc(NetworkIdentity netIdentity) =>
            _currentInteractable = netIdentity;

        public override void OnEndInteract()
        {
            if (_currentInteractable == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Current interactable is null");
#endif
                return;
            }

            if (_currentInteractable.TryGetComponent(out IInteractable interactable))
            {
                interactable.FinishInteract(this);
                ToggleInteractionEvents();
                _currentInteractable = null;
            }
        }
    }
}