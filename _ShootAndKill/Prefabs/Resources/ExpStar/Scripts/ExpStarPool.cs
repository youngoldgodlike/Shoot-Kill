using Architecture.MinePool;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Prefabs.ExpStar.Scripts
{
    public class ExpStarPool : ExpandablePool<ExpStar>
    {
        private static ExpStar _prefab;
        private static int _capacity = 20;
        private static GameObject _container;

        private static GameObject container => _container.IsUnityNull() ? _container = new GameObject("ExpStar Container") : _container; /*{
            get {
                if (_container.IsUnityNull()) _container = new GameObject("ExpStar Container");
                return _container;
            }
        }*/

    private const string PREFAB_PATH = "expStar";

        public ExpStarPool() : base(CreateStar, ReturnToPool, GetFromPool, _capacity) { }

        private static ExpStar CreateStar() {
            if (_prefab.IsUnityNull()) _prefab = Resources.Load<ExpStar>(PREFAB_PATH);

            var obj = Object.Instantiate(_prefab, container.transform);
            obj.OnReachTarget += ReturnToPool;
            return obj;
        }
        private static void ReturnToPool(ExpStar exp) => exp.gameObject.SetActive(false);
        private static void GetFromPool(ExpStar exp) => exp.gameObject.SetActive(true);

    }
}