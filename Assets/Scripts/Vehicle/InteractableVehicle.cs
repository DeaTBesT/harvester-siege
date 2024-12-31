using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Enums;
using InputModule;
using Interfaces;
using Managers;
using Mirror;
using UnityEngine;
using Vehicle.CustomSerialization;

namespace Vehicle
{
    public class InteractableVehicle : NetworkBehaviour, IInitialize, IInteractable, INetworkLoad
    {
        private const int NUMBER_PLAYER_TAKE_SEAT = 1; //Кол-во мест занимаемых одним игроков
        private const float PLAYER_COLLIDER_RADIUS = 0.5f;

        [SerializeField] private int _seatsNumber = 1;

        [SerializeField] private Transform[] _exitPositions;
        [SerializeField] private LayerMask _preventingColliderLayer;

        [SyncVar] private int _takeSeats;

        private List<NetworkIdentity> _takeSeatsObjects = new();

        private InputHandler _inputHandler;

        public int TakeSeats => _takeSeats;

        public InteractType TypeInteract => InteractType.Toggle;
        
        public NetworkIdentity NetIdentity => netIdentity;

        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }

        public bool IsEnable { get; set; }
        
        public override void OnStartServer() =>
            NetworkLoadManager.Instance.AddLoader(this);

        public override void OnStopServer() =>
            NetworkLoadManager.Instance.RemoveLoader(this);

        [Server]
        public void LoadDataServer(NetworkConnectionToClient conn)
        {
            var data = new InteractableVehicleData(_takeSeatsObjects.ToList());

            var writer = new NetworkWriter();
            writer.WriteInteractableVehicleData(data);
            var writerData = writer.ToArray();

            LoadDataRpc(conn, writerData);
        }

        [TargetRpc]
        public void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData)
        {
            var reader = new NetworkReader(writerData);
            var data = reader.ReadInteractableVehicleData();

            _takeSeatsObjects = data.Interactors;
        }

        public void Initialize(params object[] objects) =>
            _inputHandler = objects[0] as InputHandler;

        public bool TryInteract(IInteractor interactor, Action onFinishInteract = null)
        {
            if (_takeSeats >= _seatsNumber)
            {
                return false;
            }

            var interactorNetId = interactor.InteractableNetId;

            AddPlayerSeatCmd(interactorNetId);

            if (interactorNetId.TryGetComponent(out EntityController entityController))
            {
                entityController.DiactivateEntity();
            }

            if (interactorNetId.TryGetComponent(out InputHandler inputHandler))
            {
                inputHandler.SetEnableLocal(false);
            }

            if (interactorNetId.TryGetComponent(out CameraController cameraController))
            {
                cameraController.ChangeTarget(netIdentity);
            }

            OnInteract?.Invoke();

            return true;
        }

        public void StartHolding(IInteractor interactor)
        {
        }

        public void StopHolding()
        {
        }

        public void FinishInteract(IInteractor interactor)
        {
            var interactorNetId = interactor.InteractableNetId;

            if (!TryExitVehicle(out Vector2 exitPosition))
            {
                return;
            }

            RemovePlayerSeatCmd(interactorNetId);

            if (interactorNetId.TryGetComponent(out EntityController entityController))
            {
                entityController.ChangePosition(exitPosition);
                entityController.ActivateEntity();
            }

            if (interactorNetId.TryGetComponent(out InputHandler inputHandlerExitPlayer))
            {
                inputHandlerExitPlayer.SetEnableLocal(true);
            }

            if (interactorNetId.TryGetComponent(out CameraController cameraController))
            {
                cameraController.ChangeTarget(interactorNetId);
            }

            OnFinishInteract?.Invoke();
        }

        public void ForceFinishInteract(IInteractor interactor)
        {
            if (!isServer)
            {
                return;
            }

            var interactorNetId = interactor.InteractableNetId;
            ForceRemovePlayerSeatRpc(interactorNetId);

            OnFinishInteract?.Invoke();
        }

        [Command(requiresAuthority = false)]
        private void AddPlayerSeatCmd(NetworkIdentity interactorNetId) =>
            AddPlayerSeatRpc(interactorNetId);

        [ClientRpc]
        private void AddPlayerSeatRpc(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats + NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            _takeSeatsObjects.Add(interactorNetId);

            _inputHandler.SetEnableByNetId(interactorNetId, _takeSeats == 1);

            if (isServer)
            {
                if (_takeSeats <= 1)
                {
                    ChangeAuthority(interactorNetId);
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void RemovePlayerSeatCmd(NetworkIdentity interactorNetId) =>
            RemovePlayerSeatRpc(interactorNetId);

        [ClientRpc]
        private void RemovePlayerSeatRpc(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats - NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            _takeSeatsObjects.Remove(interactorNetId);

            _inputHandler.SetEnableByNetId(interactorNetId, false);

            if (_takeSeatsObjects.Count > 0)
            {
                var netIdFirst = _takeSeatsObjects.First();

                if (isServer)
                {
                    ChangeAuthority(netIdFirst);
                }

                _inputHandler.SetEnableByNetId(netIdFirst, true);
            }
        }

        [ClientRpc]
        private void ForceRemovePlayerSeatRpc(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats - NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            _takeSeatsObjects.Remove(interactorNetId);

            if (_takeSeatsObjects.Count > 0)
            {
                _inputHandler.SetEnableByNetId(_takeSeatsObjects[0], true);
            }
        }

        [Server]
        private void ChangeAuthority(NetworkIdentity newOwner)
        {
            netIdentity.RemoveClientAuthority();
            netIdentity.AssignClientAuthority(newOwner.connectionToClient);
        }

        private bool TryExitVehicle(out Vector2 exitPosition)
        {
            bool isCanExit = false;
            exitPosition = Vector2.zero;

            if (_exitPositions.Length <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("None exit positions");
#endif
                return isCanExit;
            }

            foreach (var exit in _exitPositions)
            {
                Collider2D[] collider =
                    Physics2D.OverlapCircleAll(exit.position, PLAYER_COLLIDER_RADIUS, _preventingColliderLayer);

                if (collider.Length <= 0)
                {
                    isCanExit = true;
                    exitPosition = exit.position;
                    break;
                }
            }

            return isCanExit;
        }
    }
}