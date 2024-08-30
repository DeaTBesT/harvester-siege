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
        private const int NUMBER_PLAYER_TAKE_SEAT = 1;

        [SerializeField] private int _seatsNumber = 1;

        [SyncVar] private int _takeSeats;

        private readonly SyncList<NetworkIdentity> _takeSeatsObjects = new();

        private InputHandler _inputHandler;

        public int TakeSeats => _takeSeats;

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

        public void FinishInteract(IInteractor interactor) //TODO: перемещать персонажа рядом с транспортом
        {
            var interactorNetId = interactor.InteractableNetId;

            RemovePlayerSeat(interactorNetId);

            if (interactorNetId.TryGetComponent(out EntityController entityController))
            {
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
        private void AddPlayerSeat(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats + NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            if (isServer)
            {
                _takeSeatsObjects.Add(interactorNetId);
            }

            _inputHandler.SetEnableByNetId(interactorNetId, _takeSeats == 1);
        }

        [Command(requiresAuthority = false)]
        private void RemovePlayerSeat(NetworkIdentity interactorNetId)
        {
            _takeSeats = Mathf.Clamp(_takeSeats - NUMBER_PLAYER_TAKE_SEAT, 0, _seatsNumber);

            if (isServer)
            {
                _takeSeatsObjects.Remove(interactorNetId);
            }

            _inputHandler.SetEnableByNetId(interactorNetId, false);

            if (_takeSeats > 0)
            {
                _inputHandler.SetEnableByNetId(_takeSeatsObjects[0], _takeSeats == 1);
            }
        }
    }
}