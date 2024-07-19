using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Shoot_Kill.Architecture.Scripts.Utilities
{
    public class Stopwatch
    {
        public float time { get; private set; }
        public bool isRunning { get; private set; }
        
        public event Action<float> onTick;

        public Stopwatch(float initialTime = 0)
        {
            time = initialTime;
        }

        public async UniTask StartStopwatch(CancellationToken token)
        {
            isRunning = true;
            
            while (isRunning)
            {
                time += Time.deltaTime;
                
                onTick?.Invoke(time);
                
                //Debug.Log($"Stopwatch tick {time}");
                
                await UniTask.Yield(token);
            }
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void Stop()
        {
            isRunning = false;
            time = 0;
        }
        
        
        
        
    }
}