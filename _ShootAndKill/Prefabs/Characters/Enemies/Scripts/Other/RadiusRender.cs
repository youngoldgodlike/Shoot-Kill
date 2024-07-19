using _Shoot_Kill.Prefabs.Characters.Enemies.Scripts.DmgInflictionVariety;
using NaughtyAttributes;
using UnityEngine;

namespace _Shoot_Kill.Prefabs.Characters.Enemies.Scripts
{
    public class RadiusRender : MonoBehaviour
    {
        [SerializeField] private LineRenderer _line;
        [SerializeField] private AreaAttack _areaAttack;
        [SerializeField] private bool _enableWhenAwake;
        private float radius => _areaAttack.radius;

        private void Awake() {
            if (!_enableWhenAwake) gameObject.SetActive(false);
        }

        [Button("ReDraw")]
        private void Draw() {
            DrawCircle(10, radius);
        }

        private void DrawCircle(int steps, float radic)
        {
            
            _line.positionCount = steps;

            for (int currentStep = 0; currentStep < steps; currentStep++)
            {
                var circumferenceProgress = (float) currentStep / steps;
                
                var currentRadian = circumferenceProgress * 2 * Mathf.PI;
                
                var xScaled = Mathf.Cos(currentRadian);
                var yScaled = Mathf.Sin(currentRadian);

                var x = xScaled * radic;
                var y = yScaled * radic;

                var currentInPosition = new Vector3(x,0, y);

                _line.SetPosition(currentStep, currentInPosition);
            }
        }
    }
}
