using Mirror;

namespace Interfaces
{
    public interface INetworkLoad
    {
        void LoadDataCmd(NetworkConnectionToClient conn);

        void LoadDataServer(NetworkConnectionToClient conn);

        void LoadDataRpc(NetworkConnectionToClient target, byte[] writerData);
    }
}