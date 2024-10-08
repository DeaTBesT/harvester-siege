using Core;
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

        public override void Deinitialize()
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
                    ChangeInteractionEvents();
                }
            }
        }

        private void OnMousePosition(Vector3 mousePosition) =>
            _mousePosition = mousePosition;

        public override void OnInteract()
        {
            var interactDirection = _mousePosition - (Vector2)_interactPoint.position;

            var hit = Physics2D.Raycast(_interactPoint.position,
                interactDirection,
                _interactDistance,
                _interactLayer);

            if (hit.transform == null)
            {
                return;
            }

            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.TryInteract(this))
                {
                    if (!interactable.OneTimeInteract)
                    {
                        SetCurrentInteractable(interactable.NetIdentity);
                        ChangeInteractionEvents();
                    }
                }
            }
        }

        [Command]
        private void SetCurrentInteractable(NetworkIdentity netIdentity) => 
            _currentInteractable = netIdentity;

        public override void OnEndInteract()
        {
            if (_currentInteractable.TryGetComponent(out IInteractable interactable))
            {
                interactable.FinishInteract(this);
                ChangeInteractionEvents();
            }
        }
    }
}