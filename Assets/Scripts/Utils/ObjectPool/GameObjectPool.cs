using UnityEngine;

namespace Utils.ObjectPool
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        public GameObjectPool(GameObject prefab, int preloadCount)
            : base(() => Preload(prefab), GetAction, ReturnAction, preloadCount)
        {
        }

        //TODO: создавать в пустышке весь пул объектов
        private static GameObject Preload(GameObject prefab)
        {
            GameObject @object = Object.Instantiate(prefab);
            return @object;
        }

        private static void GetAction(GameObject @object) => @object.SetActive(true);

        private static void ReturnAction(GameObject @object) => @object.SetActive(false);
    }
}