using System;
using R3;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class SpawnerController
    {
        private readonly SpawnerModel _model;
        private readonly SpawnerView _view;
        private readonly SpawnPositionFinder _positionFinder;

        private RegularSpawner _regularSpawner;
        private OnetimeSpawner _onetimeSpawner;
        private SpawnController _spawnController; // НАЕБАЛОВО! УБРАТЬ

        private SpawnerController(SpawnerModel model,SpawnerView view,SpawnPositionFinder finder) {
            Debug.Assert(model != null,"model is null");
            Debug.Assert(view != null,"view is null");
            Debug.Assert(finder != null,"position finder is null");
            _model = model;
            _view = view;
            _positionFinder = finder;

            _view.OnSpawnRequest += AddSpawnProcess;
            _view.OnSpawnCancellation += StopSpawnProcess;

            _view.Range.Subscribe(UpdateSpawnRange);
            _spawnController = _positionFinder.GetComponent<SpawnController>(); // КОСТЫЛЬ 
        }

        private void Initialize(Transform spawnPlace,Transform player) {
            _regularSpawner = new RegularSpawner(spawnPlace, player, _positionFinder, _spawnController);
            _onetimeSpawner = new OnetimeSpawner(spawnPlace, player, _positionFinder, _spawnController);
            
            _onetimeSpawner.OnRequestCompletion += RemoveRequest;
        }
        
        private void AddSpawnProcess(SpawnRequest request) {
            _model.AddRequest(request);
            InvokeSpawn(request);
        }

        private void StopSpawnProcess(SerializableGuid guid) {
            StopSpawnProcess(_model.GetRequest(guid));
            _model.RemoveRequest(guid);
        }
        
        private void StopSpawnProcess(SpawnRequest request) {
            switch (request.spawnType) {
                case SpawnType.Regular:
                    _regularSpawner.StopProcess(request);
                    break;
                case SpawnType.Onetime:
                    _onetimeSpawner.StopProcess(request);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InvokeSpawn(SpawnRequest request) {
            switch (request.spawnType) {
                case SpawnType.Regular:
                    _regularSpawner.Spawn(request);
                    break;
                case SpawnType.Onetime:
                    _onetimeSpawner.Spawn(request);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void RemoveRequest(SpawnRequest request) {
            Debug.Log("Controller Removing request");
            _model.RemoveRequest(request.guid);
            _view.RemoveRequest(request.guid);
        }

        private void UpdateSpawnRange(Vector2 range) {
            Debug.Log("SpawnRangeUpdated to " + range);
            if(_positionFinder is AroundPlayerPosition finder)
                finder.SetSpawnRange(range);
        }

        #region Builder

        public class Builder
        {
            private Transform _spawnHolder, _player;
            private SpawnerModel _model;
            private SpawnerView _view;
            private SpawnPositionFinder _positionFinder;

            public Builder Create(SpawnerView view, SpawnerModel model) {
                _model = model;
                _view = view;
                return this;
            }

            public Builder WithTarget(Transform target) {
                _player = target;
                return this;
            }

            public Builder WithSpawnHolder(Transform spawnHolder) {
                _spawnHolder = spawnHolder;
                return this;
            }

            public Builder WithSpawnPositions(SpawnPositionFinder finder) {
                _positionFinder = finder;
                return this;
            }

            public SpawnerController Build() {
                var controller = new SpawnerController(_model, _view, _positionFinder);
                
                _spawnHolder = _spawnHolder == null
                    ? new GameObject("Auto-generated Spawn Holder").transform
                    : _spawnHolder;
                _player = _player == null 
                    ? new GameObject("Auto-generated player").transform 
                    : _player;

                controller.Initialize(_spawnHolder, _player);

                Clear();
                return controller;
            }

            private void Clear() {
                _spawnHolder = null;
                _player = null;
            }
        }
        
        #endregion
    }
}