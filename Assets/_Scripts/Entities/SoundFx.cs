using UnityEngine;
using Random = System.Random;

namespace Asteroids
{
  [RequireComponent(typeof(AudioSource))]
  public class SoundFx : MonoBehaviour
  {

    [SerializeField]
    private bool _randomize = false;
    [SerializeField]
    private AudioClip[] _clips;

    private Random _random;
    private AudioSource _audio;

    private void Awake()
    {
      _random = new Random();
      _audio = GetComponent<AudioSource>();
    }

    public void Play(int index = 0, bool instanced = false)
    {
      if (_clips == null || _clips.Length < 1)
      {
        _audio.Play();

        return;
      }
        
      AudioClip clip = _clips[index];

      if (_randomize)
        clip = _clips[_random.Next(0, _clips.Length)];

      if (!instanced)
        _audio.PlayOneShot(clip);
      else
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    public void PlayInstanced(int index = 0)
    {
      Play(index, true);
    }

    public void Stop()
    {
      _audio.Stop();
    }

  }
}
