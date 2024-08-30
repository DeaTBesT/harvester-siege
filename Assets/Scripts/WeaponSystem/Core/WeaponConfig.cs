using UnityEngine;

namespace WeaponSystem.Core
{
    [CreateAssetMenu(menuName = "Weapon/New weapon config", fileName = "Weapon config")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Info")] 
        [SerializeField] private string _title = "New weapon";        
        
        [Header("Stats")]
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _fireRate = 1f;

        public string Title => _title;
        public float Damage => _damage;
        public float FireRate => _fireRate;
    }
}