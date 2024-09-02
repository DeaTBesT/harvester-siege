using Core;
using InputModule;
using Managers;
using Mirror;
using Player;
using PlayerInputModule;
using UnityEngine;

namespace PlayerModule
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerInitializer : EntityInitializer
    {
        [Header("Advanced components")] [SerializeField]
        private EntityStats _entityStats;

        [SerializeField] private EntityController _entityController;
        [SerializeField] private EntityMovementController _entityMovementController;
        [SerializeField] private EntityWeaponController _entityWeaponController;
        [SerializeField] private EntityInteractionController _entityInteractionController;
        [SerializeField] private EntityInventoryController _entityInventoryController;
        [SerializeField] private PlayerUIController _playerUIController;
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private CameraController _cameraController;

        [Header("Default components")] [SerializeField]
        private Camera _camera;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _graphics;

        protected override void OnValidate()
        {
            if (_entityStats == null)
            {
                _entityStats = GetComponent<EntityStats>();
            }

            if (_entityController == null)
            {
                _entityController = GetComponent<EntityController>();
            }

            if (_entityMovementController == null)
            {
                _entityMovementController = GetComponent<EntityMovementController>();
            }

            if (_entityWeaponController == null)
            {
                _entityWeaponController = GetComponent<EntityWeaponController>();
            }

            if (_entityInteractionController == null)
            {
                _entityInteractionController = GetComponent<EntityInteractionController>();
            } 
            
            if (_entityInventoryController == null)
            {
                _entityInventoryController = GetComponent<EntityInventoryController>();
            }

            if (_playerUIController == null)
            {
                _playerUIController = GetComponent<PlayerUIController>();
            }

            if (_inputHandler == null)
            {
                _inputHandler = GetComponent<InputHandler>();
            }

            if (_cameraController == null)
            {
                _cameraController = GetComponent<CameraController>();
            }

            if (_rigidbody2D == null)
            {
                _rigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (_collider == null)
            {
                _collider = GetComponent<Collider2D>();
            }
        }

        protected override void Initialize()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            // if (!isLocalPlayer)
            // {
            //     Destroy(_camera);
            // }

            var inputModule = new PlayerInput(_camera);
            var gameResourcesManager = GameResourcesManager.Instance;

            // ReSharper disable Unity.NoNullPropagation
            _entityStats?.Initialize();
            _inputHandler?.Initialize(inputModule);
            _inputHandler?.SetIgnoreLocalPlayer(false);
            _entityMovementController?.Initialize(inputModule,
                _rigidbody2D);
            _cameraController?.Initialize(_camera,
                transform);
            _entityWeaponController?.Initialize(inputModule,
                _entityStats);
            _entityInteractionController?.Initialize(inputModule);
            _entityController.Initialize(_entityStats,
                _entityMovementController,
                _entityWeaponController,
                _collider,
                _graphics);

            _entityInventoryController.Initialize(gameResourcesManager);
            _playerUIController?.Initialize(gameResourcesManager);
        }

        protected override void Deinitialize()
        {
            _entityMovementController?.Deinitialize();
            _entityWeaponController?.Deinitialize();
        }
    }
}