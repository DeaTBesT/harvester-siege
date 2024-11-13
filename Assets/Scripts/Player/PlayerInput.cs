using System;
using Interfaces;
using UnityEngine;

namespace PlayerInputModule
{
    public class PlayerInput : IInput
    {
        private const string HORIZONTAL_INPUT = "Horizontal";
        private const string VERTICAL_INPUT = "Vertical";

        private Camera _camera;

        private bool _onToggleEsc = false;

        public Action<Vector2> OnMove { get; set; }
        public Action<Vector3> OnMousePosition { get; set; }
        public Action OnAttackOnce { get; set; }
        public Action OnAttack { get; set; }
        public Action OnInteract { get; set; }
        public Action OnInteractUp { get; set; }
        public Action<bool> OnEscapeToggle { get; set; }

        public PlayerInput(Camera camera)
        {
            _camera = camera;

#if UNITY_EDITOR
            if (_camera == null)
            {
                Debug.LogError("Camera is null");
            }
#endif
        }

        public void MoveHandler()
        {
            var moveInput = new Vector2(Input.GetAxisRaw(HORIZONTAL_INPUT), Input.GetAxisRaw(VERTICAL_INPUT));
            OnMove?.Invoke(moveInput);
        }

        public void MouseHandler()
        {
            if (_camera == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Camera is null");
#endif
                return;
            }

            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            OnMousePosition?.Invoke(mousePosition);
        }

        public void AttackOnceHandler()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                OnAttackOnce?.Invoke();
            }
        }
        
        public void AttackHandler()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                OnAttack?.Invoke();
            }
        }

        public void InteractHandler()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteract?.Invoke();
            }
        }
        
        public void InteractUpHandler()
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                OnInteractUp?.Invoke();
            }
        }
        
        public void EscapeHandler()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _onToggleEsc = !_onToggleEsc;
                OnEscapeToggle?.Invoke(_onToggleEsc);
            }
        }
    }
}