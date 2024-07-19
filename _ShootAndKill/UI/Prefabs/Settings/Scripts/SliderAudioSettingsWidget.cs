using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.UI.Prefabs.Settings.Scripts
{
    public class SliderAudioSettingsWidget : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private string _tag;

        private void Awake()
        {
            _slider.onValueChanged.AddListener(value =>
            {
                SetVolume(value, _tag);
            });
        }

        private void Start()
        {
            _slider.value = PlayerPrefs.GetFloat($"{_tag}Volume", 0.5f);
            SetText(_slider.value);
        }

        private void SetVolume( float param, string tag)
        {
            SetText(_slider.value);
            
            var value = Mathf.Lerp(-40f, 0f, param);

            if (param != 0)
                _audioMixer.SetFloat(tag, value);
            else
                _audioMixer.SetFloat(tag, -80);
            
            PlayerPrefs.SetFloat($"{tag}Volume", param);
        }

        private void SetText( float value)
        {
            var percent = Mathf.RoundToInt(value * 100);
            _text.text = percent.ToString();
        }
    }
}
