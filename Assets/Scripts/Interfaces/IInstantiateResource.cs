namespace Interfaces
{
    public interface IInstantiateResource
    {
        public void InstantiateResource(string resourceName, int amount);
        public void InstantiateResourceCmd(string resourceName, int amount);
        public void InstantiateResourceServer(string resourceName, int amount);
    }
}