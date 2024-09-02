using Mirror;
using UnityEngine;

namespace GameResources
{
    [System.Serializable]
    public class ResourceData
    {
        [SerializeField] private GameResourceData _gameResourceData;
        [SerializeField] private int _amount;

        public GameResourceData gameResourceDataConfig => _gameResourceData;
        public int AmountResource => _amount;

        public ResourceData(GameResourceData gameResourceData, int amount)
        {
            _gameResourceData = gameResourceData;
            _amount = amount;
        }

        public void AddResource(int amount) =>
            AddResourceCmd(amount);

        [Command]
        private void AddResourceCmd(int amount) => 
            AddResourceRpc(amount);

        [ClientRpc]
        private void AddResourceRpc(int amount) => 
            _amount += amount;

        public void RemoveResource(int amount) => 
            _amount -= amount;
    }
}