using System.Collections;
using System.Collections.Generic;
using _Shoot_Kill.Architecture.Scripts.EnemySpawn;
using NaughtyAttributes;
using SpawnSystem.TestSpawner;
using UnityEngine;

public class SpawnBorderRenderer : MonoBehaviour
{
    [SerializeField] private AroundPlayerPosition _spawner;
    [SerializeField] private LineRenderer _lineIn, _lineOut;
    [SerializeField] private bool _disableOnAwake;

    private Vector2 range => _spawner.spawnRange;

    private void OnValidate() {
        DrawCircle(50, range.x, range.y);
    }
    
    private void Awake() {
        _lineIn.enabled = !_disableOnAwake;
        _lineOut.enabled = !_disableOnAwake;
    }

    public void DrawCircle(int steps, float radiusIn,float radiusOut)
    {
        _lineIn.positionCount = steps;
        _lineOut.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            var circumferenceProgress = (float) currentStep / steps;
                
            var currentRadian = circumferenceProgress * 2 * Mathf.PI;
                
            var InXScaled = Mathf.Cos(currentRadian);
            var InYScaled = Mathf.Sin(currentRadian);
            var OutXScaled = Mathf.Cos(currentRadian);
            var OutYScaled = Mathf.Sin(currentRadian);

            var xIn = InXScaled * radiusIn;
            var yIn = InYScaled * radiusIn;
            var xOut = OutXScaled * radiusOut;
            var yOut = OutYScaled * radiusOut;
                
            var currentInPosition = new Vector3(xIn,0, yIn);
            var currentOutPosition = new Vector3(xOut, 0, yOut);
            
            // if in world-space - uncomment
            // currentInPosition += transform.position;
            // currentOutPosition += transform.position;
                
            _lineIn.SetPosition(currentStep, currentInPosition);
            _lineOut.SetPosition(currentStep, currentOutPosition);
        }
    }
}
