using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Architecture.Scripts
{
    public class ProgressBarWidget : MonoBehaviour
    {
        [SerializeField] private Image _bar;
        [SerializeField] private Color _startCountdownColor;
        [SerializeField] private Color _defaultColor;
        
        public void SetProgress(float value, float maxProgress)
        {
            var progress = value / maxProgress;
            _bar.fillAmount = progress;
        }

        public async UniTaskVoid StartCountdown(float maxTime)
        {
            float timer = 0f;
            
            while (timer < maxTime)
            {
                timer += Time.deltaTime;
                var currentFillAmount = Mathf.Lerp(0f, 1f, timer / maxTime);
                _bar.fillAmount = currentFillAmount;
                
                _bar.color = Color.Lerp(_startCountdownColor, _defaultColor, currentFillAmount);
                
                await UniTask.Yield();
            }
        } 
        
        
    }
}