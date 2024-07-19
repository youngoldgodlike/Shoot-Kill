using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private Transform _grassParent;
    [SerializeField, MinMaxSlider(0, 2f)] private Vector2 _grassHeight;
    [ShowNativeProperty] private int grassCount => _grasses.Count;

    private Queue<Transform> _grasses = new();

    [Button("Hash grass transforms")]
    private async UniTaskVoid HashGrassObjects() {
        _grasses.Clear();

        var grasses = _grassParent.GetComponentsInChildren<Transform>();
        grasses = grasses.Where(x => x != transform).ToArray();
        
        foreach (var trnsfrm in grasses) {
            _grasses.Enqueue(trnsfrm);
            
            await UniTask.Yield();
        }
    }

    [Button("Clear")]
    private void ClearAll() {
        _grasses.Clear();
    }

    [Button("Rotate grass")]
    private async UniTaskVoid RotateEveryGrassRandom() {
        foreach (var grass in _grasses) {
            var rand = Random.Range(0, 360);
            grass.rotation = Quaternion.Euler(0f, rand, 0f);
            
            await UniTask.Yield();
        }
    }

    [Button("Randomize Height")]
    private async UniTask RandomizeHeight() {
        foreach (var grass in _grasses) {
            var rand = Random.Range(_grassHeight.x, _grassHeight.y);
            grass.localScale = new Vector3(1, rand, 1);
            
            await UniTask.Yield();
        }
    }
}
