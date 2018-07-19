using UnityEngine;

namespace Asteroids
{
  public class AudioManager : MonoBehaviour
  {

    private static AudioManager _instance;

    public static AudioManager Instance
    {
      get { return _instance; }
    }

    [SerializeField]
    private SoundFx _menuMusic;
    [SerializeField]
    private SoundFx _levelMusic;

    [Space]

    [SerializeField]
    private SoundFx _laserFx;
    [SerializeField]
    private SoundFx _explosionFx;

    private void Awake()
    {
      if (_instance != null)
      {
        Destroy(this);

        return;
      }

      _instance = this;
    }

    public void StopMusic()
    {
      _menuMusic.Stop();
      _levelMusic.Stop();
    }

    public void MenuMusic()
    {
      _menuMusic.Play();
    }

    public void LevelMusic()
    {
      _levelMusic.Play();
    }

    public void Laser()
    {
      _laserFx.PlayInstanced();
    }

    public void Explosion()
    {
      _explosionFx.PlayInstanced();
    }

  }
}
