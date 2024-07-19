using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundsComponent : MonoBehaviour
{
    //[SerializeField] private bool _playOneShot;
    [SerializeField] private AudioClip[] _clip;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void Play()
    {
        var random = new System.Random();
        _source.PlayOneShot(_clip[random.Next(0, _clip.Length)]);
    }
}