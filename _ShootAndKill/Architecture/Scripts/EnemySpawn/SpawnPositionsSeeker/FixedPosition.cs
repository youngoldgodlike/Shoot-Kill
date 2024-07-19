using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using R3;
using Unity.VisualScripting;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class FixedPosition : SpawnPositionFinder
    {
        [SerializeField] private Transform _positionsParent;
        [SerializeField] private GameObject _parentPrefab;
        [SerializeField] private Transform _temporaryParent;
        [SerializeField, ReadOnly] private List<SpawnPosition> _positions;

        [Header("Creating Params")] [SerializeField, Range(0.02f, 1f)]
        private float _spawnDelay = 0.2f;
        [SerializeField] private SpawnPosition _prefab;
        [SerializeField] private float _countOnWidth, _countOnHeight;
        [SerializeField] private float _yDisplacement;
        [SerializeField] private Transform _rightUp, _leftUp, _leftDown;

        [ShowNativeProperty] private int listTrueCount => _positions.Count(x => x.active);
        [ShowNativeProperty] private float diffX => (_leftUp.position -_rightUp.position).magnitude;
        [ShowNativeProperty] private float diffZ => (_leftUp.position - _leftDown.position).magnitude;
        [ShowNativeProperty]
        private float width => haveMarginalPositions
            ? diffX / (_countOnWidth - 1)
            : 0f;

        [ShowNativeProperty]
        private float height => haveMarginalPositions
            ? diffZ / (_countOnHeight - 1)
            : 0f;

        private float xStartPos => _leftUp.position.x;
        private float zStartPos => _leftDown.position.z;
        private bool haveMarginalPositions => !_leftDown.IsUnityNull() && !_leftUp.IsUnityNull() && !_rightUp.IsUnityNull();

        [Button("CachePositions")]
        private async UniTaskVoid CachePositions() {
            var hasPlayer = !player.IsUnityNull();
            Debug.Assert(hasPlayer, "player is null");
            if (!hasPlayer) return;

            _positions.Clear();
            for (int i = 0; i < _temporaryParent.childCount; i++) {
                var pos = _temporaryParent.GetChild(i).GetComponent<SpawnPosition>();
                _positions.Add(pos);

                await UniTask.Yield();
            }
        }

        private void Awake() {
            _positionsParent.gameObject.SetActive(true);
            if (_positions.IsUnityNull()) return;
            _positions.ForEach(pos => {
                pos.SetPlayer(player);
            });
        }

        public override Vector3 GetSpawnPos() {
            while (true) {
                var posList = _positions.Where(x => x.magnitude > 40f).ToList();
                var pos = posList[Random.Range(0, posList.Count - 1)].position;

                // var posList = _positions.Where(x => x.active).ToList();
                // var pos = posList[Random.Range(0, posList.Count-1)].position;
                //
                if (pos.magnitude > 38f) return pos;
                Debug.Log("GET INCORRECT POSITION");
            }
        }

        [Button("Create Spawn Positions")]
        private async UniTaskVoid CreateSpawnPoints() {
            if(_positionsParent.IsUnityNull() && haveMarginalPositions) return;
            ClearObjects();
            CreateParent();

            await UniTask.Yield();

            Debug.Log(_temporaryParent.IsUnityNull());
            for (int i = 0; i < _countOnHeight; i++) {
                for (int j = 0; j < _countOnWidth; j++) {
                    var obj = Instantiate(_prefab, _temporaryParent);
                    obj.name = $"{i}.{j} {obj.name}";
                    obj.transform.position = new Vector3(xStartPos + width * j, _yDisplacement, zStartPos + height * i);

                    await UniTask.WaitForSeconds(_spawnDelay);
                }
            }
        }

        [Button("Create Parent")]
        private void CreateParent() {
            _temporaryParent = Instantiate(_parentPrefab, _positionsParent).transform;
        }

        [Button("Clear Positions")]
        private void ClearObjects() {
            if(!_temporaryParent.IsUnityNull()) DestroyImmediate(_temporaryParent.gameObject);
            _positions.Clear();
        }
    }
}