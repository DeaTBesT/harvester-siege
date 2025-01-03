using Core;
using InputModule;
using Interfaces;
using Managers;
using Mirror;
using Player;
using Player.Enums;
using PlayerInputModule;
using UnityEngine;

namespace PlayerModule
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerInitializer : EntityInitializer, INetworkLoad
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
        private Camera _cameraPrefab;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _graphics;

        private Camera _camera;

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

        public override void OnStartServer() =>
            NetworkLoadManager.Instance.AddLoader(this);

        public override void OnStopServer() =>
            NetworkLoadManager.Instance.RemoveLoader(this);

        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn) =>
            LoadDataRpc(conn, null);

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData) =>
            Initialize();

        public override void Initialize(params object[] objects)
        {
            if (IsInitialized)
            {
                return;
            }

            if ((_camera == null) && (isLocalPlayer))
            {
                _camera = Camera.main;

                if (_camera == null)
                {
                    var cameraObject = Instantiate(_cameraPrefab, transform);
                    cameraObject.transform.parent = null;
                    cameraObject.TryGetComponent(out _camera);
                }
            }

            var inputModule = new PlayerInput(_camera);
            var gameResourcesManager = GameResourcesManager.Instance;

            // ReSharper disable Unity.NoNullPropagation
            _entityStats?.Initialize();
            _inputHandler?.Initialize(inputModule);
            _inputHandler?.SetEnableLocal(true);
            _inputHandler?.SetIgnoreLocalPlayer(false);
            _entityMovementController?.Initialize(inputModule,
                _rigidbody2D);
            _cameraController?.Initialize(_camera,
                _entityMovementController.transform);
            _entityWeaponController?.Initialize(inputModule,
                _entityStats);
            _entityInteractionController?.Initialize(inputModule);
            _entityController.Initialize(_entityStats,
                _entityMovementController,
                _entityWeaponController,
                _collider,
                _graphics);
            _entityInventoryController?.Initialize(gameResourcesManager);
            _playerUIController?.Initialize(gameResourcesManager);

            IsInitialized = true;
        }

        /// <summary>
        /// Инициализация для сервера, нужен для инициализации для других клиентов, включая самого клиента
        /// </summary>
        /// <param name="playerName"></param>
        [ClientRpc]
        public void InitializeAllRpc(string playerName)
        {
            transform.name = playerName;
            Initialize();
        }
        
        [TargetRpc]
        public void InitializeLocal(string playerName)
        {
            transform.name = playerName;
            Initialize();
        }

        public override void Deinitialize(params object[] objects)
        {
            var deinitializationState = (DeinitializationState)objects[0];

            _entityMovementController?.Deinitialize();
            _entityWeaponController?.Deinitialize();
            _entityInteractionController?.Deinitialize();
            _playerUIController?.Deinitialize();

            if (deinitializationState == DeinitializationState.Quit)
            {
                _entityInventoryController?.Deinitialize();
            }

            IsInitialized = false;
        }

        /// <summary>
        /// Деинициализация для сервера, нужен для деинициализации для других клиентов, включая самого клиента
        /// </summary>
        [ClientRpc]
        public void DeinitializeAllRpc() =>
            Deinitialize(DeinitializationState.Transition);

        [TargetRpc]
        public void DeinitializeLocal() =>
            Deinitialize(DeinitializationState.Transition);

        public override void Quit() =>
            Deinitialize(DeinitializationState.Quit);
    }
}