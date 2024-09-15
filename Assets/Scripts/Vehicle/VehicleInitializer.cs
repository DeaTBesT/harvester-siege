using Core;
using InputModule;
using Mirror;
using PlayerInputModule;
using UnityEngine;

namespace Vehicle
{
    public class VehicleInitializer : EntityInitializer
    {
        [SerializeField] private EntityMovementController _entityMovementController;
        [SerializeField] private InteractableVehicle _interactableVehicle;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Rigidbody2D _rigidbody2d;

        private Camera _camera;

        protected override void OnValidate()
        {
            if (_entityMovementController == null)
            {
                _entityMovementController = GetComponent<EntityMovementController>();
            }

            if (_interactableVehicle == null)
            {
                _interactableVehicle = GetComponent<InteractableVehicle>();
            }

            if (_inputHandler == null)
            {
                _inputHandler = GetComponent<InputHandler>();
            }

            if (_rigidbody2d == null)
            {
                _rigidbody2d = GetComponent<Rigidbody2D>();
            }
        }

        protected override void Initialize()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            var inputModule = new PlayerInput(_camera);

            _inputHandler?.Initialize(inputModule);
            _inputHandler?.SetEnableLocal(false);
            _inputHandler?.SetIgnoreLocalPlayer(true);
            _interactableVehicle?.Initialize(_inputHandler);
            _entityMovementController.Initialize(inputModule,
                _rigidbody2d,
                _interactableVehicle);

            IsInitialized = true;
        }

        protected override void Deinitialize()
        {
            _entityMovementController.Deinitialize();
            
            IsInitialized = false;
        }
    }
}