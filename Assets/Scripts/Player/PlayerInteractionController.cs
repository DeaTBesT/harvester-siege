using Core;
using Interfaces;
using UnityEngine;

namespace Player
{
    public class PlayerInteractionController : EntityInteractionController
    {
        [SerializeField] private float _interactDistance;
        [SerializeField] private LayerMask _interactLayer;
        [SerializeField] private Transform _interactPoint;

        private Vector2 _mousePosition;

        private IInteractable _currentInteractable;

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
                _currentInteractable?.ForceFinishInteract(this);
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
                        _currentInteractable = interactable;
                        ChangeInteractionEvents();
                    }
                }
            }
        }

        public override void OnEndInteract()
        {
            _currentInteractable?.FinishInteract(this);
            ChangeInteractionEvents();
        }
    }
}