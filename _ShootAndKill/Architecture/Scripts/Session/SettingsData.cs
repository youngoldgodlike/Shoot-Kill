using UnityEngine;
using UnityEngine.Audio;

namespace _Shoot_Kill.Architecture.Scripts.Session
{
    public class SettingsData : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        public AudioData audioData { get; private set; }

        private void Start() =>
            audioData = new AudioData(_audioMixer);
    }
}