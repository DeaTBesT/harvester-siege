using System;
using UnityEngine;

namespace Interfaces
{
    public interface IInput
    {
        public Action<Vector2> OnMove { get; set; }
        public Action<Vector3> OnMousePosition { get; set; }
        public Action OnAttackOnce { get; set; }
        public Action OnAttack { get; set; }
        public Action OnInteract { get; set; }
        public Action<bool> OnEscapeToggle { get; set; }
        
        public void MoveHandler();
        public void MouseHandler();
        public void AttackOnceHandler();
        public void AttackHandler();
        public void InteractHandler();
        public void EscapeHandler();
    }
}