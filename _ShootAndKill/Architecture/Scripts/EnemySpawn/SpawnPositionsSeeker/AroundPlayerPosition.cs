using System;
using System.Collections.Generic;
using NaughtyAttributes;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnSystem.TestSpawner
{
    public class AroundPlayerPosition : SpawnPositionFinder
    {
        [Header("Params")]
        [SerializeField] private LayerMask _ground;
        [SerializeField] private Vector2 _spawnRange;
        [SerializeField,Range(10f,45f)] private float _angleStep = 45f;
        [SerializeField] private float _myAngle;

        [ReadOnly, ResizableTextArea, SerializeField] private string _log;

        private Dictionary<float, bool> _spawnPossibilities;
        private List<float> _checkersAngles;
        private const int MAX_DEGREES = 359;

        public Vector2 spawnRange => _spawnRange;

        private void Awake() {
            CreateCheckers();
        }

        private void CreateCheckers() {
            _spawnPossibilities = new Dictionary<float, bool>();
            _checkersAngles = new List<float>();
            for (float i = 0; i <= MAX_DEGREES; i += _angleStep) {
                _checkersAngles.Add(i);
                _spawnPossibilities.Add(i, false);
            }
        }

        public override Vector3 GetSpawnPos() {
            var startPos = player.position;
            var randomAngle = FoundRandomRange();
            var randomRange = Random.Range(_spawnRange.x, _spawnRange.y);

            var xPos = startPos.x + randomRange * Mathf.Cos(Mathf.Deg2Rad * randomAngle);
            var zPos = startPos.z + randomRange * Mathf.Sin(Mathf.Deg2Rad * randomAngle);
            var spawnPos = new Vector3(xPos, startPos.y, zPos);

            Debug.DrawRay(startPos, spawnPos-startPos, Color.magenta, 1f);
            return spawnPos;
        }

        private float FoundRandomRange() {
            var minMaxBlock = new ReactiveProperty<Vector2>(new Vector2(-1f, 0f));

            var watcher = minMaxBlock.Subscribe(_ => {
                _log += $" Block changed to {minMaxBlock.Value.x},{minMaxBlock.Value.y}";
            });

            _log = "...";
            // нахождение минимального и максимального порога
            foreach (var angle in _checkersAngles) {
                var posib = _spawnPossibilities[angle];
                _log += $"\nAngle{angle}: {posib}.";
                    
                if (posib) continue;
                _log += $" Calculation...";

                if (angle == 0f) {
                    minMaxBlock.Value = new Vector2(angle, minMaxBlock.Value.y);
                    continue;
                }

                _log += $"\nLowest:{minMaxBlock.Value.x > angle}";
                if (minMaxBlock.Value.x == -1f) {
                    minMaxBlock.Value = minMaxBlock.Value.x < angle
                        ? new Vector2(minMaxBlock.Value.x, minMaxBlock.Value.y)
                        : new Vector2(angle, minMaxBlock.Value.y);
                }
                _log += $"\nHighest:{minMaxBlock.Value.y < angle}";
                minMaxBlock.Value = minMaxBlock.Value.y > angle
                    ? new Vector2(minMaxBlock.Value.x, minMaxBlock.Value.y)
                    : new Vector2(minMaxBlock.Value.x, angle);
            }

            var blockRange = minMaxBlock.Value.y - minMaxBlock.Value.x;
            var value = Random.Range(0f, MAX_DEGREES - blockRange);
            if (value > minMaxBlock.Value.x && value < minMaxBlock.Value.y) value += blockRange;

            _log += $"\nreturned value: {value}";
            Debug.Log($"returned value: {value}");
            watcher.Dispose();
            return value;
        }

        private void FixedUpdate() {
            IsGroundUnderneath();
            
            foreach (var angle in _checkersAngles) {
                var position = player.position;
                var xPos = position.x + _spawnRange.x * Mathf.Cos(Mathf.Deg2Rad * angle);
                var zPos = position.z + _spawnRange.x * Mathf.Sin(Mathf.Deg2Rad * angle);
                var checkPos = new Vector3(xPos, position.y, zPos);

                _spawnPossibilities[angle] = IsGroundUnderneath(checkPos);
            }
        }

        private bool IsGroundUnderneath(Vector3 pos) {
            if (Physics.Raycast(pos, -Vector3.up, 1f, _ground)) {
                Debug.DrawRay(pos, -Vector3.up, Color.green);
                return true;
            }

            Debug.DrawRay(pos, -Vector3.up, Color.red);
            return false;
        }
        
        
        private void IsGroundUnderneath() {
            var position = player.position;
            var xPos = position.x + _spawnRange.x * Mathf.Cos(Mathf.Deg2Rad * _myAngle);
            var zPos = position.z + _spawnRange.x * Mathf.Sin(Mathf.Deg2Rad * _myAngle);
            var checkPos = new Vector3(xPos, position.y, zPos);

            Debug.DrawRay(checkPos, Vector3.up * 3, Color.blue);
        }

        public void SetSpawnRange(Vector2 range) {
            _spawnRange = range;
        }
    }
}