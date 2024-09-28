using UnityEngine;

namespace Utils.ObjectPool
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        public GameObjectPool(GameObject prefab, int preloadCount, Transform entityObjectContainer)
            : base(() => Preload(prefab, entityObjectContainer), GetAction, ReturnAction, preloadCount)
        {
        }

        private static GameObject Preload(GameObject prefab, Transform parent)
        {
            var @object = Object.Instantiate(prefab, parent);
            return @object;
        }

        private static void GetAction(GameObject @object) => @object.SetActive(true);

        private static void ReturnAction(GameObject @object) => @object.SetActive(false);
    }
}