using System;

namespace Interfaces
{
    public interface IInteractable
    {
        public bool OneTimeInteract { get; }
        
        public Action OnInteract { get; set; }
        public Action OnFinishInteract { get; set; }
        
        bool TryInteract(IInteractor interactor);

        void FinishInteract(IInteractor interactor);

        void ForceFinishInteract(IInteractor interactor);
    }
}