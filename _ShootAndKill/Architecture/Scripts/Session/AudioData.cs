using UnityEngine;
using UnityEngine.Audio;

namespace _Shoot_Kill.Architecture.Scripts.Session
{
    public class AudioData
    {
        private readonly AudioMixer _audioMixer;

        public AudioData(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
            
            SetVolume("Master", PlayerPrefs.GetFloat("MasterVolume", 0.5f));
            SetVolume("Sounds", PlayerPrefs.GetFloat("SoundsVolume", 0.5f));
            SetVolume("Music", PlayerPrefs.GetFloat("MusicVolume", 0.5f));
        }
        
        private void SetVolume(string tag, float param)
        {
            var value = Mathf.Lerp(-40f, 0f, param);

            if (param != 0)
                _audioMixer.SetFloat(tag, value);
            else
                _audioMixer.SetFloat(tag, -80);
        }
    }
}