using UnityEngine;

namespace Asteroids
{
  public class GameManager : MonoBehaviour
  {

    private static GameManager _instance;

    public static GameManager Instance
    {
      get { return _instance; }
    }

    private int _score;

    public int Score
    {
      get { return _score; }
      set { _score = value; }
    }
    private void Awake()
    {
      if (_instance != null)
      {
        Destroy(this);

        return;
      }

      _instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }
  }
}
