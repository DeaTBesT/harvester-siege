using Mirror;

namespace Utils
{
    public class DontDestroyOnLoadNet : NetworkBehaviour
    {
        public override void OnStartClient() => 
            DontDestroyOnLoad(gameObject);
    }
}