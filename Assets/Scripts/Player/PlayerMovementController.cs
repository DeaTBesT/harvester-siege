using Core;
using UnityEngine;

namespace PlayerModule
{
    public class PlayerMovementController : EntityMovementController
    {
        private const float ADDED_ANGLE = 90f;

        [SerializeField] private float _speed = 5f;
        [SerializeField] private Transform _playerBody;
        
        protected override void OnMove(Vector2 moveInput)
        {
            if (!IsEnable)
            {
                return;
            }
            
            _rigidbody2d.MovePosition(_rigidbody2d.position + moveInput * _speed * Time.fixedDeltaTime);
        }

        protected override void OnMousePosition(Vector3 mousePosition)
        {
            if (!IsEnable)
            {
                return;
            }
            
            var lookDirection = (Vector2)mousePosition - _rigidbody2d.position;
            var angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - ADDED_ANGLE;

            _playerBody.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}

