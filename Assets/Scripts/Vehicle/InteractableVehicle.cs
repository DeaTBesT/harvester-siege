using System;
using Core;
using InputModule;
using Interfaces;
using Mirror;
using UnityEngine;

namespace Vehicle
{
    public class InteractableVehicle : NetworkBehaviour, IInitialize, IInteractable
    {
        private const int NUMBER_PLAYER_TAKE_SEAT = 1; //Кол-во мест занимаемых одним игроков
        private const float PLAYER_COLLIDER_RADIUS = 0.5f;

        [SerializeField] private int _seatsNumber = 1;

        [SerializeField] private Transform[] _exitPositions;
        [SerializeField] private LayerMask _preventingColliderLayer;

        [SyncVar] private int _takeSeats;

        private readonly SyncList<NetworkIdentity> _takeSeatsObjects = new();

        private InputHandler _inputHandler;

        public int TakeSeats => _takeSeats;

        public bool OneTimeInteract => false;

        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }

        public bool IsEnable { get; set; }

        public void Initialize(params object[] objects) =>
            _inputHandler = objects[0] as InputHandler;

        public bool TryInteract(IInteractor interactor)
        {
            if (_takeSeats >= _seatsNumber)
            {
                return false;
            }

            var interactorNetId = interactor.InteractableNetId;

            AddPlayerSeat(interactorNetId);

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

        public void FinishInteract(IInteractor interactor)
        {
            var interactorNetId = interactor.InteractableNetId;

            if (!TryExitVehicle(out Vector2 exitPosition))
            {
                return;
            }

            RemovePlayerSeat(interactorNetId);

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

        [Command(requiresAuthority = false)]
        private void AddPlayerSeat(NetworkIdentity interactorNetId) =>
            AddPlayerSeatRpc(interactorNetId);

        [ClientRpc]
        private void AddPlayerSeatRpc(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats + NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            if (isServer)
            {
                _takeSeatsObjects.Add(interactorNetId);
            }

            _inputHandler.SetEnableByNetId(interactorNetId, _takeSeats == 1);
        }

        [Command(requiresAuthority = false)]
        private void RemovePlayerSeat(NetworkIdentity interactorNetId) =>
            RemovePlayerSeatRpc(interactorNetId);

        [ClientRpc]
        private void RemovePlayerSeatRpc(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats - NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            if (isServer)
            {
                _takeSeatsObjects.Remove(interactorNetId);
            }

            _inputHandler.SetEnableByNetId(interactorNetId, false);

            if (_takeSeatsObjects.Count > 0)
            {
                _inputHandler.SetEnableByNetId(_takeSeatsObjects[0], true);
            }
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