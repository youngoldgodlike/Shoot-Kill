using System;
using SpawnSystem.TestSpawner;
using R3;
using UnityEngine;

public class SpawnZoneViewer : MonoBehaviour
{
    [SerializeField] private LineRenderer _externalLine, _internalLine;
    [SerializeField] private Transform _middlePoint;
    [SerializeField] private SpawnerView _view;

    [SerializeField] private int _positionsCount = 50;

    private IDisposable _disposable;
    
    private void Awake() {
        _externalLine.positionCount = _positionsCount;
        _internalLine.positionCount = _positionsCount;
        
        _view.Range.Subscribe(RefreshLines);
        var pointTracker = Observable.EveryValueChanged(this, x => x._middlePoint.position)
            .Subscribe(x => RefreshLines(_view.Range.Value));

        _disposable = Disposable.Combine(pointTracker);
    }

    private void RefreshLines(Vector2 range) {
        for (var i = 0; i < _internalLine.positionCount; i++) {
            var position = _middlePoint.position;
            var currentAngle = 360 / _positionsCount * i;
            var currentRad = Mathf.Deg2Rad * currentAngle;
            
            var inX = position.x + range.x * Mathf.Cos(currentRad);
            var inZ = position.z + range.x * Mathf.Sin(currentRad);
            var exX = position.x + range.y * Mathf.Cos(currentRad);
            var exZ = position.z + range.y * Mathf.Sin(currentRad);

            var internalPos = new Vector3(inX, position.y, inZ);
            var externalPos = new Vector3(exX, position.y, exZ);

            _internalLine.SetPosition(i, internalPos);
            _externalLine.SetPosition(i, externalPos);
        }
    }
}
