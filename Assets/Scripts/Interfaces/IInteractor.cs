using Mirror;

namespace Interfaces
{
    public interface IInteractor
    {
        NetworkIdentity InteractableNetId { get; }
        
        void OnInteract();
        void OnEndInteract();
    }
}