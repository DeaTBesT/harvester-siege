using Core;
using Mirror;
using UnityEngine;

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
            
            TakeDamageRpc(amount);
        }

        [ClientRpc]
        private void TakeDamageRpc(float amount)
        {
            Debug.Log($"Take damage: {amount}");
        }
    }
}