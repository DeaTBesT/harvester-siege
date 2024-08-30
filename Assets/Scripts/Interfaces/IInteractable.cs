using System;

namespace Interfaces
{
    public interface IInteractable
    {
        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }
        
        bool TryInteract(IInteractor interactor);

        void FinishInteract(IInteractor interactor);
    }
}