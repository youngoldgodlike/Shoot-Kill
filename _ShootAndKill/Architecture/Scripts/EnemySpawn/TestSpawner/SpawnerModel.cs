using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpawnSystem.TestSpawner
{
    public class SpawnerModel
    {
        protected readonly Dictionary<SerializableGuid, SpawnRequest> spawnRequests = new();

        public virtual void AddRequest(SpawnRequest request) {
            spawnRequests.Add(request.guid,request);
        }

        public virtual void RemoveRequest(SerializableGuid guid) {
            try {
                spawnRequests.Remove(guid);
            }
            catch (Exception e) {
                Debug.Log(e);
                throw;
            }
        }

        public virtual SpawnRequest GetRequest(SerializableGuid guid) {
            return spawnRequests[guid];
        }
    }
}