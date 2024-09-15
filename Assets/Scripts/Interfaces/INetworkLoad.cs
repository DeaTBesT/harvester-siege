using Mirror;

namespace Interfaces
{
    public interface INetworkLoad
    {
        void LoadDataServer(NetworkConnectionToClient conn);

        void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData);
    }
}