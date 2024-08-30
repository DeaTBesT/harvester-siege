using Interfaces;
using Mirror;
using UnityEngine;

namespace Core
{
    public abstract class EntityMovementController : NetworkBehaviour, IInitialize
    {
        private IInput _inputModule;
        
        protected Rigidbody2D _rigidbody2d;
        
        [field: SerializeField]  public virtual bool IsEnable { get; set; } = true;
        
        public virtual void Initialize(params object[] objects)
        {
            _inputModule = objects[0] as IInput;
            _rigidbody2d = objects[1] as Rigidbody2D;
            
            if (_inputModule != null)
            {
                _inputModule.OnMove += OnMove;
                _inputModule.OnMousePosition += OnMousePosition;
            }
        }

        public virtual void Deinitialize()
        {
            if (_inputModule != null)
            {
                _inputModule.OnMove -= OnMove;
                _inputModule.OnMousePosition -= OnMousePosition;
            }
        }

        protected abstract void OnMove(Vector2 moveInput);
        
        protected virtual void OnMousePosition(Vector3 mousePosition)
        {
            
        }
    }
}