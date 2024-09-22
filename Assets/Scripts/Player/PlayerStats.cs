using Core;
using Mirror;

namespace Player
{
    public class PlayerStats : EntityStats
    {
        public override int TeamId => 1;

        public override void TakeDamage(int teamId, float amount)
        {
            if (!IsEnable)
            {
                return;
            }
            
            TakeDamageCmd(amount);
        }

        [Command]
        private void TakeDamageCmd(float amount)
        {
            
        }

        [ClientRpc]
        private void TakeDamageRpc(float amount)
        {
            
        }
        
        public override void DestroyEntity()
        {
            
        }
    }
}