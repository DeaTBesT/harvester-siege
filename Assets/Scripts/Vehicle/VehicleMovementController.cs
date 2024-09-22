using Core;
using Mirror;
using UnityEngine;

namespace Vehicle
{
    public class VehicleMovementController : EntityMovementController
    {
        private const float RIGIDBODY_MAGNITUDE_DEVIDER = 8f;
        private const float RIGIDBODY_DRAG_IDLE = 3f;
        private const float RIGIDBODY_DRAG_DRIVE = 0f;
        private const float MAX_SPEED_MULTIPLIER = 0.5f;

        [SerializeField] private float _driftFactor = 0.95f;
        [SerializeField] private float _accelerationFactor = 20f;
        [SerializeField] private float _turnFactor = 3.5f;
        [SerializeField] private float _maxSpeed = 20f;

        [SyncVar] private float _accelerationInput = 0;
        [SyncVar] private float _steeringInput = 0;

        private float _rotationAngle = 0;
        [SyncVar] private float _velocityUp;

        private InteractableVehicle _interactableVehicle;

        public override void Initialize(params object[] objects)
        {
            base.Initialize(objects);

            _interactableVehicle = objects[2] as InteractableVehicle;

            if (_interactableVehicle != null)
            {
                _interactableVehicle.OnFinishInteract += OnFinishInteract;
            }
        }

        public override void Deinitialize()
        {
            base.Deinitialize();

            if (_interactableVehicle != null)
            {
                _interactableVehicle.OnFinishInteract -= OnFinishInteract;
            }
        }

        protected override void OnMove(Vector2 moveInput)
        {
            if (_interactableVehicle.TakeSeats <= 0)
            {
                return;
            }

            NetworkInput(moveInput.y, moveInput.x);
        }

        [Command(requiresAuthority = false)]
        private void NetworkInput(float acceleration, float steering)
        {
            if (!isServer)
            {
                return;
            }

            _accelerationInput = acceleration;
            _steeringInput = steering;
        }

        private void OnFinishInteract() =>
            NetworkInput(0, 0);

        private void FixedUpdate()
        {
            if (!isServer)
            {
                return;
            }

            ApplyEngine();
            ResetVelocity();
            ApplySteering();
        }

        private void ApplyEngine()
        {
            _velocityUp = Vector2.Dot(transform.up, _rigidbody2d.velocity);

            if ((_velocityUp > _maxSpeed) && (_accelerationInput > 0))
            {
                return;
            }

            if ((_velocityUp < -_maxSpeed * MAX_SPEED_MULTIPLIER) && (_accelerationInput < 0))
            {
                return;
            }

            if ((_rigidbody2d.velocity.sqrMagnitude > _maxSpeed * _maxSpeed) && (_accelerationInput > 0))
            {
                return;
            }

            
            _rigidbody2d.drag = _accelerationInput == 0
                ? Mathf.Lerp(_rigidbody2d.drag, RIGIDBODY_DRAG_IDLE, Time.fixedDeltaTime * 3f)
                : RIGIDBODY_DRAG_DRIVE;

            var engineForceVector = transform.up * _accelerationInput * _accelerationFactor;
            _rigidbody2d.AddForce(engineForceVector, ForceMode2D.Force);
        }

        private void ResetVelocity()
        {
            var forwardVelocity = transform.up * Vector2.Dot(_rigidbody2d.velocity, transform.up);
            var rightVelocity = transform.right * Vector2.Dot(_rigidbody2d.velocity, transform.right);

            var velocity = forwardVelocity + rightVelocity * _driftFactor;
            _rigidbody2d.velocity = velocity;
        }

        private void ApplySteering()
        {
            var minSpeedForTurning = _rigidbody2d.velocity.magnitude / RIGIDBODY_MAGNITUDE_DEVIDER;
            minSpeedForTurning = Mathf.Clamp01(minSpeedForTurning);

            var steering = _velocityUp >= 0 ? 1 : -1;
            _rotationAngle -= _steeringInput  * steering * _turnFactor * minSpeedForTurning;

            _rigidbody2d.MoveRotation(_rotationAngle);
        }
    }
}